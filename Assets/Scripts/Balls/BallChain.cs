using System.Collections.Generic;
using UnityEngine;

public sealed class BallChain
{
    private List<Ball> _balls;
    private MonoPool<Ball> _ballsPool;
    private BallPath _path;

    private Ball _head;

    private int _minDestroyIndex, _maxDestroyIndex;
    
    private bool _isEntering;
    private bool _isDestoyed;
    private int _enterIndex;
    private Vector3 _enterPos;
    private readonly float _deltaPosForEntering = 0.15f;
    private readonly float _maxDeltaBtwBalls = 1.01f;

    public BallChain(MonoPool<Ball> pool, BallPath path)
    {
        _balls = new List<Ball>();
        _ballsPool = pool;
        _path = path;
    }

    public void AddBall(Ball ball)
    {
        ball.Construct(this, _ballsPool);

        ball.transform.position = _path.HeadPosition;
        ball.FollowPath.Init(_path, _path.HeadPosition);
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

        _isEntering = true;

        int senderIndex = _balls.IndexOf(sender);

        ball.StopAllCoroutines();
        ball.Construct(this, _ballsPool);
        ball.FollowPath.Init(_path, _path.GetNearestPoint(ball.transform.position));
        ball.tag = "ChainedBall";
        ball.gameObject.layer = LayerMask.NameToLayer("ChainedBall");

        if (senderIndex == _balls.Count - 1)
        {
            _enterIndex = _balls.Count;
            _balls.Add(ball);
        }
        else
        {
            _enterIndex = side == EnterSide.Back ? senderIndex : senderIndex + 1;
            _balls.Insert(_enterIndex, ball);
        }

        _maxDestroyIndex = _minDestroyIndex = -1;
        
        int score = CheckRowScore(senderIndex);

        if (score > 0)
        {
            for (int i = _minDestroyIndex; i <= _maxDestroyIndex; i++)
            {
                _balls[i].TakeDamage(GameRules.BALL_DAMAGE);
                if (_balls[i].HP <= 0)
                {
                    _balls[i] = null;
                }
            }

            _balls.RemoveAll(item => item == null);

            _enterIndex = _minDestroyIndex;
            _isDestoyed = true;

            if (_enterIndex == _balls.Count - 1) _isEntering = false;

            EventBus.Publish<IScoreUpdateHandler>(handler => handler.OnScoreUpdate(score));
        }
        else
        {
            _isDestoyed = false;
            _enterPos = sender.transform.position;
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

            _minDestroyIndex = minIndex;
            _maxDestroyIndex = maxIndex;

            return score;
        }

        else return -1;
    }

    public void OnCastle(Ball ball)
    {
        _ballsPool.ReleaseObject(ball);
        _balls.Remove(ball);
    }

    public void MoveChain()
    {
        if (_balls == null) return;

        if (_isEntering)
        {
            if (!_isDestoyed)
            { 
                _balls[_enterIndex].transform.position = Vector3.MoveTowards
                    (
                        _balls[_enterIndex].transform.position, 
                        _enterPos,
                        Time.fixedDeltaTime * _balls[_enterIndex].FollowPath.speed
                    );

                for(int i = _enterIndex + 1; i < _balls.Count; i++)
                {
                    _balls[i].FollowPath.Move();
                }

                if ((_balls[_enterIndex].transform.position - _enterPos).magnitude < _deltaPosForEntering)
                {
                    _isEntering = false;
                }
            }
            else
            {
                for (int i = 0; i < _enterIndex; i++)
                {
                    _balls[i].FollowPath.Move();
                }
                if ((_balls[_enterIndex].transform.position - _balls[_enterIndex - 1].transform.position).magnitude <= _maxDeltaBtwBalls)
                {
                    _isEntering = false;
                }
            }
        }
        else
        {
            foreach(var ball in _balls)
            {
                ball.FollowPath.Move();
            }
        }
    }
}

public enum EnterSide
{
    Back,
    Forward
}