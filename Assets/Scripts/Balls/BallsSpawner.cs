using UnityEngine;

public sealed class BallsSpawner : MonoBehaviour, IGameStartHandler, IGameOverHandler
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private BallsPool _balls;
    [SerializeField] private Transform _ballsParent;
    [SerializeField] private AvailableColors _availableColors;
    [SerializeField] private BallPath _path;

    private BallChain _ballLine;
    private bool _enableSpawning;

    private void OnEnable()
    {
        EventBus.Subscribe(this);

        _balls.CreatePool();
        _ballLine = new BallChain(_balls.Pool, _path);

        transform.position = _path.HeadPosition;

        OnGameStart();
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);

        StopAllCoroutines();
    }

    public void OnGameStart()
    {
        _enableSpawning = true;
        Spawn();
    }

    public void OnGameOver()
    {
        _enableSpawning = false;
    }

    private void Spawn()
    {
        if (!_enableSpawning) return;

        Ball ball = _balls.Pool.PullObject();

        ball.Init(_availableColors.GetRandomColor(), "ChainedBall");

        _ballLine.AddBall(ball);
    }

    private void FixedUpdate()
    {
        _ballLine.MoveChain();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_isDebug) Debug.Log(collision.name + " exit");

        Spawn();
    }
}
