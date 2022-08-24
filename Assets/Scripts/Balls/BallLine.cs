using System.Collections.Generic;
using UnityEngine;

public sealed class BallLine
{
    private List<Ball> _balls;
    private MonoPool<Ball> _ballsPool;

    public BallLine(MonoPool<Ball> pool)
    {
        _balls = new List<Ball>();
        _ballsPool = pool;
    }

    public void AddBall(Ball ball) => _balls.Add(ball);

    public void OnBallEnter(Ball sender, Collision2D collision)
    {
        
        for(int i = 0; i < collision.contactCount; i++)
        {
            var cont = collision.GetContact(i);
        }
    }
}
