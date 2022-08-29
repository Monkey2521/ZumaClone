using System.Collections.Generic;
using UnityEngine;

public sealed class BallChain : IClearPathHandler, IGameOverHandler
{
    private List<Ball> _balls;
    private MonoPool<Ball> _ballsPool;
    private BallPath _path;
    private BallsSpawner _spawner;

    private Ball _head;

    private int _minDestroyIndex, _maxDestroyIndex, _scorePerBall;
    
    private bool _isEntering;
    private bool _isDestoyed;
    private int _enterIndex = -1;
    private Vector3 _enterPos;
    private float _enterSpeed;

    public BallChain(MonoPool<Ball> pool, BallPath path, BallsSpawner spawner)
    {
        _balls = new List<Ball>();
        _ballsPool = pool;
        _path = path;
        _spawner = spawner;

        EventBus.Subscribe(this);
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

        _enterIndex = side == EnterSide.Back ? senderIndex : senderIndex + 1;
        if (_enterIndex == _balls.Count)
        {
            _balls.Add(ball);
        }
        else
        {
            _balls.Insert(_enterIndex, ball);
        }

        ball.StopAllCoroutines();
        ball.Construct(this, _ballsPool);
        ball.tag = "ChainedBall";
        ball.gameObject.layer = LayerMask.NameToLayer("ChainedBall");

        _maxDestroyIndex = _minDestroyIndex = -1;
        
        int score = CheckRowScore(_enterIndex);

        if (score > 0)
        {
            for (int i = _minDestroyIndex; i <= _maxDestroyIndex; i++)
            {
                _balls[i].TakeDamage(GameRules.BALL_DAMAGE);
                if (_balls[i].HP <= 0)
                {
                    EventBus.Publish<IBallDestroyedHandler>(handler => handler.OnBallDestroyed(_balls[i].transform.position, _scorePerBall));

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
                Vector3 newTarget;

                _enterPos = CalculateNextBallPosition(_balls[_balls.Count - 2], out newTarget);
                _balls[_enterIndex].FollowPath.Init(_path, newTarget);
                _enterSpeed = _balls[0].FollowPath.Speed;
            }
            else if (_enterIndex == 0)
            {
                _enterPos = _balls[0].FollowPath.PreviousPoint;
                _balls[0].FollowPath.Init(_path, _balls[1].FollowPath.PreviousPoint);
                _enterSpeed = _balls[0].FollowPath.Speed;
            }
            else
            {
                _enterPos = _balls[_enterIndex + 1].transform.position;
                float enterTime = CalculateTimeToEnter(ball.transform.position, _enterPos, ball.FollowPath.Speed);
                _enterSpeed = (ball.transform.position - _enterPos).magnitude * _balls[0].FollowPath.Speed / (_balls[0].FollowPath.Speed * enterTime);
                _balls[_enterIndex].FollowPath.Init(_path, _balls[_enterIndex + 1].FollowPath.TargetPoint);
            }
            
        }
    }

    private Vector3 CalculateNextBallPosition(Ball current, out Vector3 newPathTarget)
    {
        Vector3 pos = current.transform.position;

        do
        {
            current.FollowPath.SimulateMoving(pos);
            pos = current.FollowPath.simulatedPosition;
        } while (!CheckBallsRange(pos, current.transform.position));

        newPathTarget = current.FollowPath.simulatedTarget;
        
        return pos;
    }

    private Vector3 CalculateNextBallPosition(Ball current, Ball next)
    {
        Vector3 position = current.transform.position;
        Vector3 nextPos = next.transform.position;

        if (CheckBallsRange(position, nextPos)) 
        {
            return nextPos;
        }
        else
        {
            if ((position - nextPos).magnitude < GameRules.MIN_RANGE_BTW_BALLS)
            {
                do
                {
                    current.FollowPath.SimulateMoving(position);
                    position = current.FollowPath.simulatedPosition;
                } while (!CheckBallsRange(position, current.transform.position));

                return position;
            }
            else
            {
                return Vector3.MoveTowards(nextPos, position, ((position - nextPos).magnitude - GameRules.MAX_RANGE_BTW_BALLS) / current.FollowPath.Speed);
            }
            
        }
    }

    private float CalculateTimeToEnter(Vector3 from, Vector3 to, float speed)
    {
        float time = 0;

        Vector3 position = from;

        while ((to - position).magnitude > GameRules.MAX_DELTA_POS)
        {
            position = Vector3.MoveTowards(position, to, Time.fixedDeltaTime * speed);
            time += Time.fixedDeltaTime;
        }

        return time;
    }

    private bool CheckBallsRange(Vector3 first, Vector3 second) => 
        (second - first).magnitude <= GameRules.MAX_RANGE_BTW_BALLS && 
        (second - first).magnitude >= GameRules.MIN_RANGE_BTW_BALLS;

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
            _scorePerBall = (ballsRowLength - GameRules.MIN_ROW_TO_DESTROY) * GameRules.ADDITIONAL_SCORE_PER_BALL + GameRules.SCORE_PER_BALL;

            _minDestroyIndex = minIndex;
            _maxDestroyIndex = maxIndex;

            return _scorePerBall * ballsRowLength;
        }

        else return -1;
    }

    public void OnCastle(Ball ball)
    {
        if (_balls.IndexOf(ball) == _enterIndex)
        {
            _isEntering = false;
            _isDestoyed = false;
            _enterIndex = -1;
            _enterSpeed = _balls[0].FollowPath.Speed;
            _enterPos = -Vector3.one;
        }

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
                        Time.fixedDeltaTime * _enterSpeed
                    );

                for(int i = _enterIndex + 1; i < _balls.Count; i++)
                {
                    _balls[i].FollowPath.Move();
                }

                if ((_balls[_enterIndex].transform.position - _enterPos).magnitude < GameRules.MAX_DELTA_POS)
                {
                    _isEntering = false;
                    _isDestoyed = false;
                    _enterIndex = -1;
                    _enterSpeed = _balls[0].FollowPath.Speed;
                    _enterPos = -Vector3.one;
                }
            }
            else
            {
                for (int i = _enterIndex; i < _balls.Count; i++)
                {
                    _balls[i].FollowPath.MoveBack(_balls[i].FollowPath.Speed * GameRules.MOVE_BACK_ON_PATH_SPEED_MULTIPLIER);
                }

                for (int i = _enterIndex + 1; i < _balls.Count; i++)
                {
                    if (!CheckBallsRange(_balls[i].transform.position, _balls[i - 1].transform.position))
                        _balls[i].transform.position = CalculateNextBallPosition(_balls[i - 1], _balls[i]);
                }

                if ((_balls[_enterIndex].transform.position - _balls[_enterIndex - 1].transform.position).magnitude <= GameRules.MAX_RANGE_BTW_BALLS)
                {
                    _isEntering = false;
                    _isDestoyed = false;
                    _enterIndex = -1;
                    _enterSpeed = _balls[0].FollowPath.Speed;
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

            for (int i = 1; i < _balls.Count; i++)
            {
                if (!CheckBallsRange(_balls[i].transform.position, _balls[i - 1].transform.position))
                    _balls[i].transform.position = CalculateNextBallPosition(_balls[i - 1], _balls[i]);
            }
        }
    }

    public void OnClearPath()
    {
        while(_balls.Count > 0)
        {
            _ballsPool.ReleaseObject(_balls[_balls.Count - 1]);
            _balls.RemoveAt(_balls.Count - 1);
        }

        _spawner.OnGameStart();
    }

    public void OnGameOver()
    {
        while (_balls.Count > 0)
        {
            _ballsPool.ReleaseObject(_balls[_balls.Count - 1]);
            _balls.RemoveAt(_balls.Count - 1);
        }
    }

    ~BallChain (){
        EventBus.Unsubscribe(this);
    }
}

public enum EnterSide
{
    Back,
    Forward
}