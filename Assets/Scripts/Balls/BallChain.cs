using System.Collections.Generic;
using UnityEngine;

public sealed class BallChain
{
    private List<Ball> _balls;
    private MonoPool<Ball> _ballsPool;
    private BallPath _path;

    public BallChain(MonoPool<Ball> pool, BallPath path)
    {
        _balls = new List<Ball>();
        _ballsPool = pool;
        _path = path;
    }

    public void AddBall(Ball ball)
    {
        ball.Construct(this);

        _balls.Insert(0, ball);
    }

    public void OnBallEnter(Ball sender, Ball ball, EnterSide side)
    {
        if (!_balls.Contains(sender))
        {
            Debug.Log("Missing ball!");

            return;
        }

        int senderIndex = _balls.IndexOf(sender);

        ball.Construct(this);
      
        _balls.Insert(side == EnterSide.Back ? senderIndex : senderIndex + 1, ball);
    }

    public void MoveChain()
    {

    }
}

public enum EnterSide
{
    Back,
    Forward
}