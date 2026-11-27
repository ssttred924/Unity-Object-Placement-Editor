using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

/// <summary>
/// 負責場景物件的拖曳放置管理。
/// 建立物件預覽、透過 Raycast 偵測地形位置，
/// 並在拖曳結束時決定正式放置或取消放置。
/// 同時調整物件高度，使物件底部能貼合地面。
/// </summary>
public class PlacementManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera _mainCamera;

    [Header("Terrain")]
    [SerializeField] private GameObject _environment;
    [SerializeField] private LayerMask _hittableLayer;

    [Header("Guide Line")]
    [SerializeField] private LineRenderer _guideLine;

    [Header("Preview")]
    [SerializeField] private float _previewDistance = 5f;
    [SerializeField] private Vector2 previewOffset = new Vector2(-100f, 100f);
    [SerializeField] private float _previewObjMoveSpeed = 100f;

    [Header("XR Preview")]
    [SerializeField] private Transform _xrPreviewAnchor;

    private GameObject _previewObject;
    private GameObject _currentPrefab;

    private bool _isDragging;
    private bool _hasValidPosition;

    private Vector3 _currentPlacementPosition;
    private Vector3 _currentPlacementNormal;
    private Vector3 _previewWorldPosition;

    //單例模式（Singleton）
    public static PlacementManager Instance { get; private set; }

    private void Awake()
    {
        //檢查是否已實例此物件，是:摧毀目前物件，否:靜態記憶體指向此物件
        if (Instance != null)
        {
            Debug.Log("More than one PlacementManager active in the scene! Destroying lastest one: " + name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (_guideLine == null)
            Debug.LogError("_guideLine is missing.");

        if(_environment == null)
            Debug.LogError("_guideLine is missing.");
    }

    private void Update()
    {
        if (_previewObject == null)
            return;

        _previewObject.transform.position =
            Vector3.Lerp(_previewObject.transform.position, _previewWorldPosition, _previewObjMoveSpeed * Time.deltaTime);

        _guideLine.SetPosition(0, _previewObject.transform.position);
    }

    public void BeginDrag(GameObject prefab, Vector2 screenPosition)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Prefab is missing.");
            return;
        }

        CancelDrag();

        _currentPrefab = prefab;

        // 取得預覽物件在攝影機前方的位置
        Vector3 previewScreenPosition =
            new Vector3(screenPosition.x + previewOffset.x, screenPosition.y + previewOffset.y, _previewDistance);

        _previewWorldPosition = _mainCamera.ScreenToWorldPoint(previewScreenPosition);

        _previewObject = Instantiate(prefab, _previewWorldPosition,Quaternion.identity);

        foreach (Collider collider in _previewObject.GetComponentsInChildren<Collider>(true))
            collider.enabled = false;

        _isDragging = true;
        _hasValidPosition = false;

        // 啟用指引線
        _guideLine.startColor = UnityEngine.Color.red;
        _guideLine.endColor = UnityEngine.Color.red;

    }

    public void UpdateDrag(Vector2 screenPosition,int pointerID)
    {
        if (!_isDragging || _previewObject == null)
            return;

        // 取得預覽物件在攝影機前方的位置
        Vector3 previewScreenPosition =
            new Vector3(screenPosition.x + previewOffset.x, screenPosition.y + previewOffset.y, _previewDistance);

        _previewWorldPosition = _mainCamera.ScreenToWorldPoint(previewScreenPosition);

        // 從滑鼠位置向場景發射射線
        Ray ray = _mainCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, _hittableLayer))
        {
            _guideLine.SetPosition(1, hit.point);

            //滑鼠若位於 UI 上，取消放置
            if (!EventSystem.current.IsPointerOverGameObject(pointerID))
                _guideLine.enabled = true;
            else
                _guideLine.enabled = false;

            if (hit.transform.CompareTag("Terrain"))
            {
                // 記錄真正的地形落點
                _currentPlacementPosition = hit.point;
                _currentPlacementNormal = hit.normal;
                _hasValidPosition = true;

                _guideLine.startColor = UnityEngine.Color.green;
                _guideLine.endColor = UnityEngine.Color.green;
            }
            else
            {
                _guideLine.startColor = UnityEngine.Color.red;
                _guideLine.endColor = UnityEngine.Color.red;
                _hasValidPosition = false;
            }

        }
        else
        {
            _hasValidPosition = false;

            // 沒有有效落點時隱藏指引線
            _guideLine.enabled = false;
        }
    }

    public void EndDrag(bool cancel)
    {
        if (!_isDragging)
            return;

        if (!cancel && _hasValidPosition && _currentPrefab != null)
        {
            GameObject placedObject = Instantiate(_currentPrefab);

            AlignObjectToGround(placedObject, _currentPlacementPosition, _currentPlacementNormal);

            placedObject.transform.SetParent(_environment.transform, true);
        }

        CancelDrag();
    }

    /// 根據地面法線計算物件旋轉，
    /// 使物件的 Up 方向與地面法線一致。
    private void AlignObjectToGround(GameObject target, Vector3 groundPosition, Vector3 groundNormal)
    {
        target.transform.position = groundPosition;

        // 將物件原本的 Forward 投影到地面平面
        Vector3 forward = Vector3.ProjectOnPlane(target.transform.forward, groundNormal).normalized;

        // 避免 Forward 與地面法線平行時無法計算旋轉
        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.ProjectOnPlane(_currentPrefab.transform.right, groundNormal).normalized;

        //讓物件跟隨地面法線旋轉
        target.transform.rotation = Quaternion.LookRotation(forward, groundNormal);
    }

    public void CancelDrag()
    {
        if (_previewObject != null)
        {
            Destroy(_previewObject);
        }

        _guideLine.enabled = false;

        _previewObject = null;
        _currentPrefab = null;

        _isDragging = false;
        _hasValidPosition = false;
    }

    /// <summary>
    /// 更新 VR 模式下預覽物件的位置。
    /// </summary>
    public void UpdateXRPreviewPosition()
    {
        if (_xrPreviewAnchor == null || _previewObject == null)
            return;

        _previewWorldPosition = _xrPreviewAnchor.position;
    }

    /// <summary>
    /// 開始 VR 模式下的道具拖曳。
    /// </summary>
    public void BeginXRDrag(GameObject prefab)
    {
        if (prefab == null || _xrPreviewAnchor == null)
        {
            Debug.LogWarning("Prefab or XR Preview Anchor is missing.");
            return;
        }

        CancelDrag();

        _currentPrefab = prefab;

        _previewWorldPosition = _xrPreviewAnchor.position;
        _previewObject = Instantiate(prefab, _previewWorldPosition, Quaternion.identity);

        foreach (Collider collider in _previewObject.GetComponentsInChildren<Collider>(true))
            collider.enabled = false;

        _isDragging = true;
        _hasValidPosition = false;

        _guideLine.SetPosition(0, _previewObject.transform.position);
        _guideLine.SetPosition(1, _previewObject.transform.position);

        _guideLine.startColor = Color.red;
        _guideLine.endColor = Color.red;

        _guideLine.enabled = true;
    }

    /// <summary>
    /// 使用 VR 控制器射線偵測地形，更新道具落點與指引線。
    /// </summary>
    public void UpdateXRDrag(Ray ray, bool IsGetUIRaycast)
    {
        if (!_isDragging || _previewObject == null)
            return;

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, _hittableLayer))
        {
            _guideLine.enabled = true;
            _guideLine.SetPosition(1, hit.point);

            if (hit.transform.CompareTag("Terrain") && !IsGetUIRaycast)
            {
                _currentPlacementPosition = hit.point;
                _currentPlacementNormal = hit.normal;

                _hasValidPosition = true;

                _guideLine.startColor = Color.green;
                _guideLine.endColor = Color.green;
            }
            else
            {
                _hasValidPosition = false;

                _guideLine.startColor = Color.red;
                _guideLine.endColor = Color.red;
            }
        }
        else
        {
            _hasValidPosition = false;
            _guideLine.enabled = false;
        }
    }
}
