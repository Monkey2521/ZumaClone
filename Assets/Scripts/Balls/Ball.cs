using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] Rigidbody2D _rigidbody;

    protected Color _color;
    private BallLine _line;

    public Vector2 MoveDirection => _rigidbody.velocity;

    public void Construct(BallLine line)
    {
        _line = line;
    }

    public void Init(Color color)
    {
        _color = color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBall")
        {
            _line.OnBallEnter(this, collision);
        }
        else return;
    }
}
