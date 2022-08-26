using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField, Range(0f, 0.1f)] private float _deltaPosForNextPoint;
    public float speed;

    private Vector3 _targetPoint;

    private BallPath _path;

    public Vector3 TargetPoint => _targetPoint;

    public void Init(BallPath path)
    {
        _path = path;
        _targetPoint = -Vector3.one;
    }

    public void Move()
    {
        if (_targetPoint == -Vector3.one)
        {
            _targetPoint = _path.HeadPosition;
        }

        transform.position = Vector3.MoveTowards(transform.position, _targetPoint, Time.fixedDeltaTime * speed);

        if ((transform.position - _targetPoint).magnitude < _deltaPosForNextPoint)
        {
            _targetPoint = _path.GetNextPoint(_targetPoint);
        }
    }
}
