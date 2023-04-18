using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Systems;
using UnityEngine;

public class LevelBuildingSystem : BaseSystem
{
    [BoxGroup("Level Editing")]
    [SerializeField] private LevelData _currentLevelData;
    
    [Foldout("Prefabs")] 
    [SerializeField] private GameObject _cornerPrefab;
    [Foldout("Prefabs")] 
    [SerializeField] private GameObject _wallPrefab;
    [Foldout("Prefabs")] 
    [SerializeField] private GameObject _centerPrefab;
    [Foldout("Prefabs")] 
    [SerializeField] private ChipContainer _chipContainerPrefab;

    [BoxGroup("Builder Settings")] 
    [SerializeField] private float _blockDistance = 1;
    [BoxGroup("Builder Settings")] 
    [SerializeField] private int _cornerDefaultRotation = 0;
    [BoxGroup("Builder Settings")]
    [SerializeField] private int _wallDefaultRotation = 0;
    [BoxGroup("Builder Settings")]
    [SerializeField] private float _planeContainerYOffset;

    private List<GameObject> _objects = new();

    public override void Init()
    {
        BuildLevel(_currentLevelData);
    }

    [Button]
    private void BuildCurrentLevel()
    {
        BuildLevel(_currentLevelData);
    }
    
    public List<List<Vector3>> BuildLevel(LevelData levelData)
    {
        ClearLevel();
        BuildCorners(levelData);
        BuildWalls(levelData);
        var positions = BuildBlocks(levelData);
        BuildChipContainers(levelData);
        return positions;
    }

    private List<List<Vector3>> BuildBlocks(LevelData levelData)
    {
        var result = new List<List<Vector3>>();
        for (int x = 0; x < levelData.Width; x++)
        {
            var tempResult = new List<Vector3>();
            
            for (int z = 0; z < levelData.Length; z++)
            {
                var block = BuildBlock(x, z);
                tempResult.Add(block.transform.position + Vector3.up * _planeContainerYOffset);
            }   
            result.Add(tempResult);
        }

        return result;
    }

    private GameObject BuildBlock(int x, int z)
    {
        var obj = Instantiate(
            _centerPrefab,
            GetPositionAtIndex(x, z),
            Quaternion.identity, transform);
        obj.name = $"(X: {x}, Z: {z})";
        _objects.Add(obj);
        return obj;
    }

    private void BuildWalls(LevelData levelData)
    {
        for (int x = 0; x < levelData.Width; x++)
        {
            BuildWall(x, -1, 0);
            BuildWall(x, levelData.Length, 180);
        }

        for (int z = 0; z < levelData.Length; z++)
        {
            BuildWall(-1, z, 90);
            BuildWall(levelData.Width, z, 270);
        }
    }

    private void BuildWall(int x, int z, int angle)
    {
        var obj = Instantiate(_wallPrefab,
            GetPositionAtIndex(x, z),
            Quaternion.Euler(0, _wallDefaultRotation + angle, 0), transform);
        _objects.Add(obj);
    }

    private void BuildCorners(LevelData levelData)
    {
        BuildCorner(-1, -1, 0);
        BuildCorner(-1, levelData.Length, 90);
        BuildCorner(levelData.Width, levelData.Length, 180);
        BuildCorner(levelData.Width, -1, 270);
    }

    private void BuildCorner(int x, int z, int angle)
    {
        var obj = Instantiate(_cornerPrefab,
            GetPositionAtIndex(x, z),
            Quaternion.Euler(0, _cornerDefaultRotation + angle, 0), transform);
        _objects.Add(obj);
    }

    private void BuildChipContainers(LevelData levelData)
    {
        foreach (var planeContainerData in levelData.ChipContainersData)
        {
            BuildChipContainer(planeContainerData);
        }
    }

    private void BuildChipContainer(ChipContainerData chipContainerData)
    {
        var obj = Instantiate(_chipContainerPrefab.gameObject, transform);
        obj.transform.position = GetPositionAtIndex(chipContainerData.X, chipContainerData.Z)
                                 + Vector3.up * _planeContainerYOffset;
        obj.GetComponent<ChipContainer>().Setup(chipContainerData.Size);
        _objects.Add(obj);
    }

    private Vector3 GetPositionAtIndex(int x, int z)
    {
        return new Vector3(x * _blockDistance, transform.position.y, z * _blockDistance);
    }

    [Button]
    private void ClearLevel()
    {
        _objects.ForEach(obj =>
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        });
        _objects.Clear();
    }
}