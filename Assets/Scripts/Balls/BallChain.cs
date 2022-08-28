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
    private int _enterIndex = -1;
    private Vector3 _enterPos;

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
        ball.tag = "ChainedBall";
        ball.gameObject.layer = LayerMask.NameToLayer("ChainedBall");

        _enterIndex = side == EnterSide.Back ? senderIndex : senderIndex + 1;
        if (_enterIndex == _balls.Count)
        {
            _balls.Add(ball);
        }
        else
        {
            _balls.Insert(_enterIndex, ball);
        }

        _maxDestroyIndex = _minDestroyIndex = -1;
        
        int score = CheckRowScore(_enterIndex);

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

            if (_enterIndex > _balls.Count - 1)
            {
                _isEntering = false;
                _isDestoyed = false;
                _enterIndex = -1;

                _head = _balls[_balls.Count - 1];

                Debug.Log("Destroy head");
            }

            EventBus.Publish<IScoreUpdateHandler>(handler => handler.OnScoreUpdate(score));
        }
        else
        {
            _isDestoyed = false;

            if (_enterIndex == _balls.Count - 1)
            {
                _enterPos = Vector3.MoveTowards
                    (
                        _balls[_enterIndex - 1].transform.position,
                        _balls[_enterIndex - 1].transform.position + _balls[_enterIndex - 1].FollowPath.MoveDirection * GameRules.MAX_RANGE_BTW_BALLS,
                        GameRules.MAX_RANGE_BTW_BALLS
                    );
            }
            else if (_enterIndex == 0)
            {
                _enterPos = _balls[0].FollowPath.PreviousPoint;
                _balls[0].FollowPath.Init(_path, _balls[1].FollowPath.PreviousPoint);
            }
            else
            {
                _enterPos = _balls[_enterIndex + 1].transform.position;
                _balls[_enterIndex].FollowPath.Init(_path, _balls[_enterIndex + 1].FollowPath.TargetPoint);
            }
            
        }
    }

    private Vector3 CalculateNextBallPosition(Vector3 currentPos)
    {
        Vector3 pos = Vector3.one;

        return pos;
    }

    private Vector3 CalculateNextBallPosition(Vector3 current, Vector3 next)
    {
        Vector3 pos = Vector3.one;

        return pos;
    }

    private bool CheckBallsRange(Vector3 first, Vector3 second) => (second - first).magnitude < GameRules.MAX_RANGE_BTW_BALLS;

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
                        Time.fixedDeltaTime * _balls[_enterIndex].FollowPath.Speed
                    );

                for(int i = _enterIndex + 1; i < _balls.Count; i++)
                {
                    _balls[i].FollowPath.Move();
                }

                if ((_balls[_enterIndex].transform.position - _enterPos).magnitude < GameRules.MAX_DELTA_POS)
                {
                    _isEntering = false;
                }
            }
            else
            {
                for (int i = _enterIndex; i < _balls.Count; i++)
                {
                    _balls[i].FollowPath.MoveBack(_balls[i].FollowPath.Speed * GameRules.MOVE_BACK_ON_PATH_SPEED_MULTIPLIER);
                }
                if ((_balls[_enterIndex].transform.position - _balls[_enterIndex - 1].transform.position).magnitude <= GameRules.MAX_RANGE_BTW_BALLS)
                {
                    _isEntering = false;
                    _isDestoyed = false;
                    _enterIndex = -1;
                    _enterPos = -Vector3.one;
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