
using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Systems;
using UnityEngine;

public class SceneContext : MonoBehaviour
{
    private static SceneContext s_instance;
    
    [ReorderableList]
    [SerializeField] private List<BaseSystem> _systems; 
    
    private void Awake()
    {
        if (s_instance == null) s_instance = this;
        else if (s_instance != this) Destroy(gameObject);
        
        _systems.ForEach(s => s.Init());
    }

    public T Get<T>() where T : BaseSystem
    {
        var result = _systems.Find(s => s.GetType() == typeof(T));
        if (result == null && isActiveAndEnabled)
        {
            throw new NullReferenceException($"System of type {typeof(T)} is not implemented!");
        }
        return (T)result;
    }
    
    public static SceneContext GetInstance()
    {
        return s_instance;
    }

    public static T GetSystem<T>() where T : BaseSystem
    {
        var instance = GetInstance();
        return instance == null ? null : instance.Get<T>();
    }
}