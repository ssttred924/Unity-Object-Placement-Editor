
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 負責控制場景相機的移動、旋轉、平移與縮放。
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 10f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 0.2f;
    [SerializeField] private float _minPitch = -80f;
    [SerializeField] private float _maxPitch = 80f;

    [Header("Pan")]
    [SerializeField] private float _panSpeed = 0.01f;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 10f;

    private float _pitch;
    private float _yaw;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;

        _pitch = angles.x;
        _yaw = angles.y;
    }
    private void Update()
    {
        HandleMovement();

        // 滑鼠位於 UI 上時，不執行滑鼠相機操作。
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        HandleRotation();
        HandlePan();
        HandleZoom();
    }

    /// <summary>
    /// 使用 WASD、Q、E 移動相機。
    /// </summary>
    private void HandleMovement()
    {
        Vector3 direction = Vector3.zero;

        if (Keyboard.current.wKey.isPressed)
            direction += transform.forward;

        if (Keyboard.current.sKey.isPressed)
            direction -= transform.forward;

        if (Keyboard.current.dKey.isPressed)
            direction += transform.right;

        if (Keyboard.current.aKey.isPressed)
            direction -= transform.right;

        if (Keyboard.current.eKey.isPressed)
            direction += Vector3.up;

        if (Keyboard.current.qKey.isPressed)
            direction -= Vector3.up;

        if (direction.sqrMagnitude > 0f)
        {
            transform.position += direction.normalized * _moveSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// 按住滑鼠右鍵旋轉相機。
    /// </summary>
    private void HandleRotation()
    {
        if (!Mouse.current.rightButton.isPressed)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        _yaw += mouseDelta.x * _rotationSpeed;
        _pitch -= mouseDelta.y * _rotationSpeed;

        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    /// <summary>
    /// 按住滑鼠中鍵平移相機。
    /// </summary>
    private void HandlePan()
    {
        if (!Mouse.current.middleButton.isPressed)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        Vector3 movement = (-transform.right * mouseDelta.x + -transform.up * mouseDelta.y) * _panSpeed;

        transform.position += movement;
    }

    /// <summary>
    /// 使用滑鼠滾輪縮放相機。
    /// </summary>
    private void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        transform.position += transform.forward * scroll * _zoomSpeed * Time.deltaTime;
    }
}
