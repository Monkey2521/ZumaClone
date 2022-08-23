using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    Color _color;

    public void Init(Color color)
    {
        _color = color;
    }
}
