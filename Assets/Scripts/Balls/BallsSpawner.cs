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

    private BallChain _ballChain;
    [HideInInspector] public bool enableSpawning;

    private void OnEnable()
    {
        EventBus.Subscribe(this);

        _balls.CreatePool();
        _ballChain = new BallChain(_balls.Pool, _path, this);

        transform.position = _path.HeadPosition;
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);

        StopAllCoroutines();
    }

    public void OnGameStart()
    {
        enableSpawning = true;
        Spawn();
    }

    public void OnGameOver()
    {
        enableSpawning = false;
    }

    private void Spawn()
    {
        if (!enableSpawning) return;

        Ball ball = _balls.Pool.PullObject();

        ball.Init(_availableColors.GetRandomColor(), "ChainedBall");

        _ballChain.AddBall(ball);
    }

    private void FixedUpdate()
    {
        _ballChain.MoveChain();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_isDebug) Debug.Log(collision.name + " exit");

        Spawn();
    }
}
