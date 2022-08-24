using UnityEngine;

public sealed class Castle : MonoBehaviour, IDamageable, IGameStartHandler
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private ObjectStats _stats;

    private int _hp;
    public int MaxHP => _stats.MaxHP;
    public int HP => _hp;

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    public void OnGameStart()
    {
        _hp = MaxHP;
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;

        EventBus.Publish<IPlayerHPUpdateHandler>(handler => handler.OnPlayerHPUpdate(HP, MaxHP));

        if (_hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        EventBus.Publish<IGameOverHandler>(handler => handler.OnGameOver());
    }
}
