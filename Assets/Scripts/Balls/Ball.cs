using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private FollowPath _followPath;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SoundList _sounds;
    [SerializeField, Range(0.5f, 10f)] private float _speed = 1f;

    protected Color _color;
    private BallChain _line;

    private MonoPool<Ball> _pool;
    private float _releaseDelay;

    public FollowPath FollowPath => _followPath;
    public float Speed => _speed;
    public Color Color => _color;
    public Rigidbody2D Rigidbody => _rigidbody;

    public void Construct(BallChain line)
    {
        _line = line;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBall" && _line != null)
        {
            if (_isDebug) Debug.Log("Collision with player ball");

            Ball enterBall = collision.gameObject.GetComponent<Ball>();

            _line.OnBallEnter(this, enterBall, GetEnterSide(enterBall.transform.position));
        }
        else if (collision.gameObject.tag == "Castle" && tag != "PlayerBall")
        {
            if (_isDebug) Debug.Log("Collision with castle");

            collision.gameObject.GetComponent<Castle>().TakeDamage(1);
        }
        else return;
    }
    
    private EnterSide GetEnterSide(Vector3 enterPoint)
    {
        EnterSide side;

        if (true)
        {
            side = EnterSide.Forward;
        }
        else
        {
            side = EnterSide.Back;
        }

        return side;
    }
}
