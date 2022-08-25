using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] private SoundList _sounds;

    protected Color _color;
    private BallChain _line;

    private MonoPool<Ball> _pool;
    private float _releaseDelay;

    public Rigidbody2D Rigidbody => _rigidbody;
    public Vector2 MoveDirection => _rigidbody.velocity.normalized;

    public void Construct(BallChain line)
    {
        _line = line;
        _pool = null;
        _releaseDelay = 0f;
    }

    public void Construct (MonoPool<Ball> pool, float releaseDelay)
    {
        _pool = pool;
        _releaseDelay = releaseDelay;
        _line = null;
    }

    public void Init(Color color, string tag)
    {
        _color = color;
        _renderer.color = _color;

        transform.tag = tag;
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
            Ball enterBall = collision.gameObject.GetComponent<Ball>();

            _line.OnBallEnter(this, enterBall, GetEnterSide(enterBall.transform.position));
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
