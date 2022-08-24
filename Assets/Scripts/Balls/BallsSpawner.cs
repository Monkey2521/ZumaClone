using UnityEngine;

public sealed class BallsSpawner : MonoBehaviour, IGameStartHandler, IGameOverHandler
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private Ball _ballPrefab;
    [SerializeField, Range(1, 1000)] private int _ballsPoolCapacity;
    [SerializeField] private Transform _ballsParent;
    
    [SerializeField] private AvailableColors _availableColors;

    [SerializeField] private BallPath _path;

    private MonoPool<Ball> _ballsPool;
    private BallLine _ballLine;

    private void OnEnable()
    {
        EventBus.Subscribe(this);

        if (_ballsPool != null)
        {
            _ballsPool.ClearPool();
        }

        _ballsPool = new MonoPool<Ball>(_ballPrefab, _ballsPoolCapacity, _ballsParent);
        _ballLine = new BallLine(_ballsPool);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);

        StopAllCoroutines();
    }

    public void OnGameStart()
    {

    }

    public void OnGameOver()
    {

    }

    private void Spawn()
    {
        Ball ball = _ballsPool.PullObject();

        ball.Init(_availableColors.GetRandomColor());

        // TODO add to ballLine
    }
}
