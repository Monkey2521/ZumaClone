using UnityEngine;

public class TowerPlayer : MonoBehaviour, IScreenTapHandler
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private BallsPool _balls;

    private Ball _currentBall;
    private Ball _nextBall;

    public Ball NextBall => _nextBall;

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    private void Start()
    {
        if (_currentBall == null)
        {
            _currentBall = _balls.Pool.PullObject();
        }

        if (_nextBall == null)
        {
            _nextBall = _balls.Pool.PullObject();
        }
    }

    public void OnScreenTap(Vector3 point)
    {

    }
}
