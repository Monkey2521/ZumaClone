using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour, IDamageable
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private ObjectStats _stats;
    [SerializeField] private FollowPath _followPath;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SoundList _sounds;
    [SerializeField, Range(0.5f, 10f)] private float _speed = 1f;

    protected Color _color;
    private BallChain _line;

    private MonoPool<Ball> _pool;
    private float _releaseDelay;

    private int _hp;

    public int MaxHP => _stats.MaxHP; // TODO возможность появления "тяжелых" шаров,
    public int HP => _hp; // для уничтожения которых нужно два или более попаданий (три в ряд)
    public FollowPath FollowPath => _followPath;
    public float Speed => _speed;
    public Color Color => _color;
    public Rigidbody2D Rigidbody => _rigidbody;

    [HideInInspector] public Vector2 moveDirection;
    [HideInInspector] public bool waitChain = true;

    public void Construct(BallChain line, MonoPool<Ball> pool)
    {
        _line = line;
        _pool = pool;
        _rigidbody.velocity = Vector2.zero;
    }

    public void Construct (MonoPool<Ball> pool, float releaseDelay)
    {
        _pool = pool;
        _releaseDelay = releaseDelay;
        _line = null;
        _rigidbody.velocity = Vector2.zero;
    }

    public void Init(Color color, string tag)
    {
        _color = color;
        _renderer.color = _color;

        this.tag = tag;
    }

    public void Throw(Vector2 force)
    {
        _rigidbody.AddForce(force, ForceMode2D.Impulse);

        moveDirection = force;

        StartCoroutine(ReleaseDelay(_releaseDelay));
    }

    private IEnumerator ReleaseDelay(float time)
    {
        yield return new WaitForSeconds(time);

        if (_line == null)
        {
            _pool.ReleaseObject(this);
        }
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;

        if (_hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        _pool.ReleaseObject(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _rigidbody.velocity = Vector2.zero;

        if (collision.gameObject.tag == "PlayerBall" && _line != null)
        {
            if (_isDebug) Debug.Log("Collision with player ball");

            Ball enterBall = collision.gameObject.GetComponent<Ball>();

            _line.OnBallEnter(this, enterBall, GetEnterSide(enterBall));
        }
        else if (collision.gameObject.tag == "Castle" && !CompareTag("PlayerBall"))
        {
            if (_isDebug) Debug.Log("Collision with castle");

            collision.gameObject.GetComponent<Castle>().TakeDamage(GameRules.BALL_DAMAGE);

            _pool.ReleaseObject(this);
        }
        else if (CompareTag(collision.gameObject.tag) && CompareTag("ChainedBall"))
        {
            waitChain = false;
        } 
        else return;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _rigidbody.velocity = Vector2.zero;

        if (CompareTag(collision.gameObject.tag) && CompareTag("ChainedBall"))
        {
            waitChain = true;
        }
    }

    private EnterSide GetEnterSide(Ball enterBall)
    {
        EnterSide side;

        Vector2 selfPos = transform.position;
        Vector2 enterPos = enterBall.transform.position;

        if ((selfPos.x < enterPos.x && selfPos.y < enterPos.y && moveDirection.y < moveDirection.x) ||  // down right ->
            (selfPos.x > enterPos.x && selfPos.y > enterPos.y && moveDirection.y > moveDirection.x) ||  // up left    <-
            (selfPos.x < enterPos.x && selfPos.y > enterPos.y && moveDirection.y <= moveDirection.x) || // left down   v
            (selfPos.x > enterPos.x && selfPos.y < enterPos.y && moveDirection.y >= moveDirection.x))   //right up     ^
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
