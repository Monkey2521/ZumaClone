using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField, Range(0.5f, 10f)] private float _speed;

    private Vector3 _previousPoint;
    private Vector3 _targetPoint;
    private Vector3 _moveDirection;

    private BallPath _path;

    public Vector3 TargetPoint => _targetPoint;
    public Vector3 PreviousPoint => _previousPoint;
    public Vector3 MoveDirection => (_moveDirection - transform.position).normalized;
    public float Speed => _speed;

    public Vector3 simulatedPosition;
    public Vector3 simulatedTarget;

    /// <summary>
    /// Initializing following path
    /// </summary>
    /// <param name="path">Path reference</param>
    /// <param name="target">Set -Vector3.one or path.HeadPosition, if need start from head</param>
    public void Init(BallPath path, Vector3 target)
    {
        _path = path;
        _targetPoint = target;
        _previousPoint = _path.GetPreviousPoint(_targetPoint);
        GetMoveDirection(_speed);
    }

    public void Move()
    {
        if (_targetPoint == -Vector3.one)
        {
            _targetPoint = _path.HeadPosition;
            _previousPoint = -Vector3.one;
        }

        GetMoveDirection(_speed);
        transform.position = _moveDirection;

        if (CheckNextPoint(transform.position))
        {
            _previousPoint = _targetPoint;
            _targetPoint = _path.GetNextPoint(_targetPoint);
            simulatedTarget = _targetPoint;
        }
    }

    public void MoveBack(float speed)
    {
        if (_previousPoint == -Vector3.one)
        {
            _previousPoint = _path.HeadPosition;
            _targetPoint = _path.GetNextPoint(_previousPoint);
        }

        GetMoveDirection(speed, movingBack: true);
        transform.position = _moveDirection;

        if (CheckNextPoint(transform.position, movingBack: true))
        {
            _targetPoint = _previousPoint;
            _previousPoint = _path.GetPreviousPoint(_previousPoint);
            simulatedTarget = _targetPoint;
        }
    }

    public void SimulateMoving(Vector3 from)
    {
        if (simulatedTarget == -Vector3.one)
        {
            simulatedTarget = _path.HeadPosition;
        }

        simulatedPosition = Vector3.MoveTowards(from, simulatedTarget, Time.fixedDeltaTime * _speed);

        if ((simulatedPosition - simulatedTarget).magnitude < GameRules.MAX_DELTA_POS )
        {
            simulatedTarget = _path.GetNextPoint(simulatedTarget);
        }
    }

    /// <summary>
    /// Check if next path point is needed
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool CheckNextPoint(Vector3 position, bool movingBack = false)
    {
        return ((movingBack ? _previousPoint : _targetPoint) - position).magnitude < GameRules.MAX_DELTA_POS;
    }

    private void GetMoveDirection(float speed, bool movingBack = false)
    {
        _moveDirection = Vector3.MoveTowards(transform.position, movingBack ? _previousPoint : _targetPoint, Time.fixedDeltaTime * speed);
    }
}
