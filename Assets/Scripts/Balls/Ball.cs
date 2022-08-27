using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour, IDamageable
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private ObjectStats _stats;
    [SerializeField] private FollowPath _followPath;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SoundList _sounds;

    protected Color _color;
    private BallChain _chain;

    private MonoPool<Ball> _pool;
    private float _releaseDelay;
    private bool _isThrowed;

    private int _hp;

    public int MaxHP => _stats.MaxHP; // TODO возможность появления "тяжелых" шаров,
    public int HP => _hp; // для уничтожения которых нужно два или более попаданий (три в ряд)
    public FollowPath FollowPath => _followPath;
    public Color Color => _color;

    public void Construct(BallChain chain, MonoPool<Ball> pool)
    {
        _chain = chain;
        _pool = pool;
        _hp = MaxHP;
    }

    public void Construct (MonoPool<Ball> pool, float releaseDelay)
    {
        _pool = pool;
        _releaseDelay = releaseDelay;
        _chain = null;
    }

    public void Init(Color color, string tag)
    {
        _color = color;
        _renderer.color = _color;

        this.tag = tag;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void Throw(Vector3 direction, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction * speed, speed * Time.fixedDeltaTime);

        if (!_isThrowed) StartCoroutine(WaitRelease());

        StartCoroutine(WaitThrow(direction, speed));

        _isThrowed = true;
    }

    private IEnumerator WaitThrow(Vector3 direction, float speed)
    {
        yield return new WaitForFixedUpdate();
        Throw(direction, speed);
    }

    private IEnumerator WaitRelease()
    {
        yield return new WaitForSeconds(_releaseDelay);
        _pool.ReleaseObject(this);
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;

        if (_hp <= 0)
        {
            Die();
            EventBus.Publish<ISoundPlayHandler>(handler => handler.OnSoundPlay(_sounds[SoundNames.Destroy]));
        }
        else
        {
            EventBus.Publish<ISoundPlayHandler>(handler => handler.OnSoundPlay(_sounds[SoundNames.TakeDamage]));
        }
    }

    public void Die()
    {
        _pool.ReleaseObject(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBall" && _chain != null)
        {
            if (_isDebug) Debug.Log("Collision with player ball");

            Ball enterBall = collision.gameObject.GetComponent<Ball>();

            _chain.OnBallEnter(this, enterBall, GetEnterSide(enterBall));
        }
        else if (collision.gameObject.tag == "Castle" && !CompareTag("PlayerBall"))
        {
            if (_isDebug) Debug.Log("Collision with castle");

            collision.gameObject.GetComponent<Castle>().TakeDamage(GameRules.BALL_DAMAGE);

            _chain.OnCastle(this);
        }
        else return;
    }

    private EnterSide GetEnterSide(Ball enterBall)
    {
        EnterSide side;

        Vector2 selfPos = transform.position;
        Vector2 enterPos = enterBall.transform.position;

        if ((selfPos.x < enterPos.x && selfPos.y < enterPos.y) || // down right ->
            (selfPos.x > enterPos.x && selfPos.y > enterPos.y) || // up left    <-
            (selfPos.x < enterPos.x && selfPos.y > enterPos.y) || // left down   v
            (selfPos.x > enterPos.x && selfPos.y < enterPos.y))   //right up     ^
        {
            side = EnterSide.Forward;

            if (_isDebug) Debug.Log("Enter forward: " + GetInstanceID());
        }
        else
        {
            side = EnterSide.Back;

            if (_isDebug) Debug.Log("Enter back: " + GetInstanceID());
        }

        return side;
    }
}
