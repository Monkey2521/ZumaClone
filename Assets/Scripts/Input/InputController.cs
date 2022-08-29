using UnityEngine;

public class InputController : MonoBehaviour
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private Camera _camera;
    
    private bool _isMobile;
    private Vector3 _tapPosition;

    public bool IsMobile => _isMobile;
    public Vector3 TapPosition => _tapPosition;

    private void OnEnable()
    {
        _isMobile = Application.isMobilePlatform;
    }

    private void OnDisable()
    {
        
    }

    void Update()
    {
        _tapPosition = Vector3.one;

        if (_isMobile)
        {
            if (Input.touchCount > 0)
            {
                _tapPosition = _camera.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                _tapPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        if (_tapPosition != Vector3.one)
        {
            if (_isDebug) Debug.Log("Tap on " + _tapPosition);
            
            _tapPosition.z = 0;
            EventBus.Publish<IScreenTapHandler>(handler => handler.OnScreenTap(_tapPosition));          
        }
    }
}
