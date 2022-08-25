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

    private BallLine _ballLine;

    private void OnEnable()
    {
        EventBus.Subscribe(this);

        _balls.CreatePool();
        _ballLine = new BallLine(_balls.Pool, _path);
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
        Ball ball = _balls.Pool.PullObject();

        ball.Init(_availableColors.GetRandomColor(), "Untagged");

        _ballLine.AddBall(ball);
    }

    private void FixedUpdate()
    {
        _ballLine.MoveLine();
    }
}
