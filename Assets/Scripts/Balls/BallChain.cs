using System.Collections.Generic;
using UnityEngine;

public sealed class BallChain
{
    private List<Ball> _balls;
    private MonoPool<Ball> _ballsPool;
    private BallPath _path;

    private Ball _head;

    public BallChain(MonoPool<Ball> pool, BallPath path)
    {
        _balls = new List<Ball>();
        _ballsPool = pool;
        _path = path;
    }

    public void AddBall(Ball ball)
    {
        ball.Construct(this);

        ball.transform.position = _path.HeadPosition;
        ball.FollowPath.Init(_path, ball.Speed);
        ball.gameObject.layer = LayerMask.NameToLayer("ChainedBall");

        _balls.Insert(0, ball);

        if (_head == null)
        {
            _head = ball;
        }
    }

    public void OnBallEnter(Ball sender, Ball ball, EnterSide side)
    {
        if (!_balls.Contains(sender))
        {
            Debug.Log("Missing ball!");

            return;
        }

        sender.Rigidbody.velocity = Vector2.zero;

        int senderIndex = _balls.IndexOf(sender);

        ball.StopAllCoroutines();
        ball.Construct(this);
        ball.FollowPath.Init(_path, ball.Speed);
        ball.FollowPath.InitTarget(sender.FollowPath.TargetPoint);
        ball.tag = "ChainedBall";
        ball.gameObject.layer = LayerMask.NameToLayer("ChainedBall");

        if (senderIndex == _balls.Count - 1)
        {
            _balls.Add(ball);
        }
        else
        {
            _balls.Insert(side == EnterSide.Back ? senderIndex : senderIndex + 1, ball);
        }

        if (CheckRowScore(senderIndex) > 0)
        {
            // TODO destoy balls
        }
    }

    private int CheckRowScore(int index)
    {
        Color currentColor = _balls[index].Color;
        int maxIndex = index, minIndex = index;

        int currentIndex = index;
        while (currentIndex > 0)
        {
            if (_balls[currentIndex - 1].Color == currentColor)
            {
                currentIndex--;
                minIndex = currentIndex;
            }
            else break;
        }

        currentIndex = index;
        while (currentIndex < _balls.Count - 1)
        {
            if (_balls[currentIndex + 1].Color == currentColor)
            {
                currentIndex++;
                maxIndex = currentIndex;
            }
            else break;
        }

        int ballsRowLength = maxIndex - minIndex + 1;

        if (ballsRowLength >= GameRules.MIN_ROW_TO_DESTROY)
        {
            int score = 0;

            score += ballsRowLength * GameRules.SCORE_PER_BALL;
            score += (ballsRowLength - GameRules.MIN_ROW_TO_DESTROY) * GameRules.ADDITIONAL_SCORE_PER_BALL * ballsRowLength;

            return score;
        }

        else return -1;
    }

    public void MoveChain()
    {
        if (_balls == null) return;

        foreach(var ball in _balls)
        {
            ball.FollowPath.Move();
        }
    }
}

public enum EnterSide
{
    Back,
    Forward
}