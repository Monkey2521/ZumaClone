using System.Collections.Generic;
using UnityEngine;

public sealed class BallPath : MonoBehaviour
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;
    [SerializeField] private Color _gizmosColor;

    [Header("Settings")]
    [Tooltip("Calculates in childrens from contex menu")]
    [SerializeField] private List<Vector3> _points = new List<Vector3>();

    public List<Vector3> Points => _points;
    public Vector3 HeadPosition => _points[0];

    /// <summary>
    /// Get next point in path
    /// </summary>
    /// <param name="point">Path point to get next</param>
    /// <returns>Next path point</returns>
    public Vector3 GetNextPoint(Vector3 point)
    {
        if (!_points.Contains(point))
        {
            return -Vector3.one;
        }

        int index = _points.IndexOf(point);

        if (index == _points.Count - 1) return -Vector3.one;
        else return _points[index + 1];
    }

    /// <summary>
    /// Get prev point in path
    /// </summary>
    /// <param name="point">Path point to get prev</param>
    /// <returns>Previuos path point</returns>
    public Vector3 GetPreviousPoint(Vector3 point)
    {
        if (!_points.Contains(point))
        {
            return -Vector3.one;
        }

        int index = _points.IndexOf(point);

        if (index == 0) return -Vector3.one;
        else return _points[index - 1];
    }

    /// <summary>
    /// Calculate path points in chidren transforms
    /// </summary>
    [ContextMenu("Calculate points")]
    private void CalculatePoints()
    {
        _points = new List<Vector3>();

        for(int i = 0; i < transform.childCount; i++)
        {
            _points.Add(transform.GetChild(i).position);
        }
    }

    private void OnDrawGizmos()
    {
        if (_isDebug)
        {
            Gizmos.color = _gizmosColor;

            for (int i = 1; i < _points.Count; i++)
            {
                Gizmos.DrawLine(_points[i - 1], _points[i]);
            }
        }
    }
}