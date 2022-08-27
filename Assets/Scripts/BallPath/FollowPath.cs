using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField, Range(0f, 0.1f)] private float _deltaPosForNextPoint;
    [SerializeField, Range(0.5f, 10f)] private float _speed;

    private Vector3 _targetPoint;
    private Vector3 _moveDirection;

    private BallPath _path;

    public Vector3 TargetPoint => _targetPoint;
    public Vector3 MoveDirection => (_moveDirection - transform.position).normalized;
    public float Speed => _speed;

    /// <summary>
    /// Initializing following path
    /// </summary>
    /// <param name="path">Path reference</param>
    /// <param name="target">Set -Vector3.one or path.HeadPosition, if need start from head</param>
    public void Init(BallPath path, Vector3 target)
    {
        _path = path;
        _targetPoint = target;
        GetMoveDirection();
    }

    public void Move()
    {
        if (_targetPoint == -Vector3.one)
        {
            _targetPoint = _path.HeadPosition;
        }

        GetMoveDirection();
        transform.position = _moveDirection;

        if (CheckNextPoint(transform.position))
        {
            _targetPoint = _path.GetNextPoint(_targetPoint);
        }
    }

    /// <summary>
    /// Check if next path point is needed
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool CheckNextPoint(Vector3 position)
    {
        return (_targetPoint - position).magnitude < _deltaPosForNextPoint;
    }

    private void GetMoveDirection()
    {
        _moveDirection = Vector3.MoveTowards(transform.position, _targetPoint, Time.fixedDeltaTime * _speed);
    }
}
