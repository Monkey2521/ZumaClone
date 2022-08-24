using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unsorted/BallsPool", fileName = "New balls pool")]
public class BallsPool : ScriptableObject
{
    [SerializeField] private Ball _ballPrefab;
    [SerializeField, Range(1, 1000)] private int _ballsPoolCapacity;
    
    private MonoPool<Ball> _pool;

    public MonoPool<Ball> Pool => _pool;
    public bool Initialized => _pool != null;

    public void CreatePool()
    {
        if (Initialized)
        {
            _pool.ClearPool();
        }

        Transform poolParent = new GameObject("BallsPool").transform;

        _pool = new MonoPool<Ball>(_ballPrefab, _ballsPoolCapacity, poolParent);
    }
}
