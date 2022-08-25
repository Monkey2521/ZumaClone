using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField, Range(0f, 0.1f)] private float _deltaPosForNextPoint;

    private Vector3 _targetPoint;

    private BallPath _path;
    private float _speed;

    public Vector3 TargetPoint => _targetPoint;

    public void Init(BallPath path, float speed)
    {
        _path = path;
        _speed = speed;
        _targetPoint = -Vector3.one;
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

        //float speed = (transform.position - _targetPoint).magnitude < _deltaPosForNextPoint ? _speed + 1 : _speed;
        transform.position = Vector3.MoveTowards(transform.position, _targetPoint, Time.fixedDeltaTime * _speed);

        if ((transform.position - _targetPoint).magnitude < _deltaPosForNextPoint)
        {
            _targetPoint = _path.GetNextPoint(_targetPoint);
        }
    }
}
