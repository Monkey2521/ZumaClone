using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField, Range(0f, 0.1f)] private float _deltaPosForNextPoint;
    [SerializeField] private Ball _ball;

    private Vector3 _prevTarget;
    private Vector3 _targetPoint;

    private BallPath _path;
    private float _speed;

    public Vector3 TargetPoint => _targetPoint;

    public void Init(BallPath path, float speed)
    {
        _path = path;
        _speed = speed;
        _targetPoint = -Vector3.one;
        _prevTarget = -Vector3.one;
    }

    public void InitTarget(Vector3 point)
    {
        _targetPoint = point;
    }

    public void Move()
    {
        if (_targetPoint == -Vector3.one)
        {
            _targetPoint = _path.HeadPosition;
        }

        Vector3 moveTo = Vector3.MoveTowards(transform.position, _targetPoint, Time.fixedDeltaTime * _speed);

        _ball.moveDirection = (moveTo - transform.position).normalized;

        transform.position = moveTo;

        if ((transform.position - _targetPoint).magnitude < _deltaPosForNextPoint)
        {
            _prevTarget = _targetPoint;
            _targetPoint = _path.GetNextPoint(_targetPoint);
        }
    }

    public void MoveBack()
    {
        if (_prevTarget == -Vector3.one)
        {
            _prevTarget = _path.HeadPosition;
        }

        Vector3 moveTo = Vector3.MoveTowards(transform.position, _prevTarget, Time.fixedDeltaTime * _speed);

        _ball.moveDirection = (moveTo - transform.position).normalized;

        transform.position = moveTo;

        if ((transform.position - _prevTarget).magnitude < _deltaPosForNextPoint)
        {
            _targetPoint = _prevTarget;
            _prevTarget = _path.GetPrevPoint(_prevTarget);
        }
    }
}
