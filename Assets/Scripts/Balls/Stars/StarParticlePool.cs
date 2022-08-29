using UnityEngine;

public class StarParticlePool : MonoBehaviour, IBallDestroyedHandler
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Pool settings")]
    [SerializeField] private StarParticle _particlePrefab;
    [SerializeField, Range(1, 10)] private int _poolCapacity;

    private MonoPool<StarParticle> _pool;
    public bool Initialized => _pool != null;

    private void OnEnable()
    {
        EventBus.Subscribe(this);

        CreatePool();
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }


    public void CreatePool()
    {
        if (Initialized)
        {
            _pool.ClearPool();
        }

        Transform poolParent = new GameObject("StarParticlePool").transform;

        _pool = new MonoPool<StarParticle>(_particlePrefab, _poolCapacity, poolParent);
    }

    public void OnBallDestroyed(Vector3 position, int scorePerBall)
    {
        var obj = _pool.PullObject();

        obj.Construct(_pool);

        obj.transform.position = position;
        obj.Play(scorePerBall);
    }
}
