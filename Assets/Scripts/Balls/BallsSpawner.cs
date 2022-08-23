using UnityEngine;

public sealed class BallsSpawner : MonoBehaviour
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private Ball _ballPrefab;
    [SerializeField, Range(1, 1000)] private int _ballsPoolCapacity;
    [SerializeField] private Transform _ballsParent;
    
    [SerializeField] private AvailableColors _availableColors;

    private MonoPool<Ball> _ballsPool;

    private void OnEnable()
    {
        //EventBus.Subscribe(this);

        if (_ballsPool != null)
        {
            _ballsPool.ClearPool();
        }

        _ballsPool = new(_ballPrefab, _ballsPoolCapacity, _ballsParent);
    }

    private void OnDisable()
    {
        //EventBus.Unsubscribe(this);
    }
}
