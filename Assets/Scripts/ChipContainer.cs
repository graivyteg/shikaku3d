using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ChipContainer : MonoBehaviour
{
    [SerializeField] private Chip _chipPrefab;
    [SerializeField] private float _chipDistance = 0.5f;

    private List<Chip> _chips = new();

    private const float FallAnimationTime = 0.7f;
    private const float FallHeight = 5;
    private const float FallDelay = 0.1f;

    private void Start()
    {
        PlayStartAnimation();
    }

    public void Setup(int size)
    {
        ClearChips();
        
        for (int i = 0; i < size; i++)
        {
            var chip = Instantiate(_chipPrefab.gameObject, transform);
            chip.transform.position = transform.position + Vector3.up * i * _chipDistance;
            _chips.Add(chip.GetComponent<Chip>());
        }
    }

    private void PlayStartAnimation()
    {
        for (int i = 0; i < _chips.Count; i++)
        {
            var defaultPosition = _chips[i].transform.position;
            _chips[i].transform.position += Vector3.up * FallHeight;
            
            _chips[i].transform.DOScale(Vector3.one, FallAnimationTime).SetDelay(FallDelay * i);
            _chips[i].transform.DOMove(defaultPosition, FallAnimationTime).SetDelay(FallDelay * i);
        }
    }

    private void ClearChips()
    {
        _chips.ForEach(chip => DestroyImmediate(chip));   
    }
}