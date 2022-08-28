using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public sealed class Castle : MonoBehaviour, IDamageable, IGameStartHandler, IPointerClickHandler
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private ObjectStats _stats;
    [SerializeField] private SoundList _sounds;
    [SerializeField] private ClearBooster _clearBooster;
    [SerializeField] private Image _boosterColldownImage;

    private bool _onGame;
    private bool _boosterReady;
    private float _timer;

    private int _hp;
    public int MaxHP => _stats.MaxHP;
    public int HP => _hp;

    private void OnEnable()
    {
        EventBus.Subscribe(this);

        OnGameStart();
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    public void OnGameStart()
    {
        _hp = MaxHP;
        _timer = 0;
        _onGame = true;
        _boosterReady = false;
    }

    private void Update()
    {
        if (!_boosterReady && _onGame)
        {
            _boosterColldownImage.fillAmount = _timer / _clearBooster.Cooldown;

            if (_timer >= _clearBooster.Cooldown)
            {
                _boosterReady = true;
                EventBus.Publish<ISoundPlayHandler>(handler => handler.OnSoundPlay(_sounds[SoundNames.Reload]));
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (_isDebug) Debug.Log("Castle take damage: hp = " + _hp + ", damage = " + damage);

        _hp -= damage;

        EventBus.Publish<IPlayerHPUpdateHandler>(handler => handler.OnPlayerHPUpdate(HP, MaxHP));
        
        if (_hp <= 0)
        {
            Die();
        }
        else
        {
            EventBus.Publish<ISoundPlayHandler>(handler => handler.OnSoundPlay(_sounds[SoundNames.TakeDamage]));
        }
    }

    public void Die()
    {
        _onGame = false;

        EventBus.Publish<ISoundPlayHandler>(handler => handler.OnSoundPlay(_sounds[SoundNames.Destroy]));
        EventBus.Publish<IGameOverHandler>(handler => handler.OnGameOver());

        if (_isDebug) Debug.Log("Game over");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isDebug) Debug.Log("Click on Castle");

        if (_boosterReady && _onGame)
        {
            EventBus.Publish<ISoundPlayHandler>(handler => handler.OnSoundPlay(_sounds[SoundNames.UseBooster]));
            _clearBooster.MakeEffect();
            _boosterReady = false;
            _timer = 0;
        }
    }
}
