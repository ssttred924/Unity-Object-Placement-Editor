
using UnityEngine;

/// <summary>
/// 負責讓 VR 道具清單 Canvas 固定在 XR Camera 視野右上角。
/// </summary>
public class VRPropListPositionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _xrCamera;

    [Header("Settings")]
    [SerializeField] private float _distance = 1.5f;

    [Tooltip("距離視野右上角的比例偏移")]
    [SerializeField] private Vector2 _margin = new Vector2(0.03f, 0.03f);

    private RectTransform _canvasRect;

    private void Awake()
    {
        _canvasRect = GetComponent<RectTransform>();

        // 以 Canvas 右上角作為定位基準
        _canvasRect.pivot = new Vector2(1f, 1f);
    }

    private void LateUpdate()
    {
        if (_xrCamera == null)
            return;

        // 取得相機視野右上角稍微往內縮的位置
        Vector3 viewportPosition = new Vector3(
            1f - _margin.x,
            1f - _margin.y,
            _distance
        );

        // 轉換成世界座標
        Vector3 worldPosition =
            _xrCamera.ViewportToWorldPoint(viewportPosition);

        // 移動整個 Canvas
        transform.position = worldPosition;
        transform.rotation = _xrCamera.transform.rotation;
    }
}
