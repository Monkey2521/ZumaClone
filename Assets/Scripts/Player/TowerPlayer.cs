using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlayer : MonoBehaviour, IScreenTapHandler
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private BallsPool _balls;
    [SerializeField] private AvailableColors _availableColors;
    [SerializeField] private List<Sound> _sounds;

    [Header("Throw settings")]
    [SerializeField, Range(1f, 10f)] private float _ballThrowSpeed;
    [SerializeField, Range(0f, 1.5f)] private float _reloadTime;
    [SerializeField, Range(0f, 10f)] private float _releaseDelay;

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
            _currentBall.transform.position = transform.position;
            _currentBall.transform.parent = transform;

            _currentBall.Init(_availableColors.GetRandomColor(), "PlayerBall");
        }

        if (_nextBall == null)
        {
            _nextBall = _balls.Pool.PullObject();
            _nextBall.gameObject.SetActive(false);
            _nextBall.transform.parent = transform;

            _nextBall.Init(_availableColors.GetRandomColor(), "PlayerBall");
        }
    }

    private IEnumerator GetNextBall()
    {
        _currentBall = null;

        yield return new WaitForSeconds(1);

        _currentBall = _nextBall;
        _currentBall.gameObject.SetActive(true);
        _currentBall.transform.position = transform.position;

        _nextBall = _balls.Pool.PullObject();
        _nextBall.gameObject.SetActive(false);
        _nextBall.transform.parent = transform;

        _nextBall.Init(_availableColors.GetRandomColor(), "PlayerBall");
    }

    public void OnScreenTap(Vector3 point)
    {
        if (_currentBall != null)
        {
            _currentBall.transform.parent = null;

            _currentBall.Construct(_balls.Pool, _releaseDelay);
            _currentBall.Throw(point.normalized * _ballThrowSpeed);

            StartCoroutine(GetNextBall());
        }
    }
}
