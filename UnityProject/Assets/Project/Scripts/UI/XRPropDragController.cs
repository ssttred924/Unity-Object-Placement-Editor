
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// 負責處理 VR 模式下的道具拖曳操作。
/// </summary>
public class XRPropDragController : MonoBehaviour
{
    [Header("XR Controller")]
    [SerializeField] private Transform _rightController;
    [SerializeField] private XRRayInteractor _rayInteractor;

    [Header("Input")]
    [SerializeField] private InputActionReference _selectAction;

    [Header("Placement")]
    [SerializeField] private PlacementManager _placementManager;

    private GameObject _currentPrefab;
    private bool _isDragging;

    private void OnEnable()
    {
        _selectAction.action.Enable();

        _selectAction.action.started += OnTriggerPressed;
        _selectAction.action.canceled += OnTriggerReleased;
    }

    private void OnDisable()
    {
        _selectAction.action.started -= OnTriggerPressed;
        _selectAction.action.canceled -= OnTriggerReleased;

        _selectAction.action.Disable();
    }

    private void Update()
    {
        if (!_isDragging)
            return;

        // 更新預覽物件在控制器左上方的位置
        _placementManager.UpdateXRPreviewPosition();

        // 判斷射線是否命中 UI
        bool isGetUIRaycast = _rayInteractor.TryGetCurrentUIRaycastResult(out _);

        // 從右手控制器射線偵測地形
        Ray ray = new Ray(_rightController.position, _rightController.forward);
        _placementManager.UpdateXRDrag(ray, isGetUIRaycast);
    }

    private GameObject GetTargetPrefab()
    {
        if (_rayInteractor == null)
            return null;

        if (!_rayInteractor.TryGetCurrentUIRaycastResult(out var result))
            return null;

        PropDragItem propItem = result.gameObject.GetComponentInParent<PropDragItem>();

        if (propItem == null)
            return null;

        return propItem.GetPrefab();
    }

    /// <summary>
    /// 按下 Trigger，開始拖曳道具。
    /// </summary>
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (_isDragging)
            return;

        _currentPrefab = GetTargetPrefab();

        if (_currentPrefab == null)
            return;

        _placementManager.BeginXRDrag(_currentPrefab);

        _isDragging = true;
    }

    /// <summary>
    /// 放開 Trigger，正式放置道具。
    /// </summary>
    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        if (!_isDragging)
            return;

        Ray ray = new Ray(_rightController.position, _rightController.forward);

        // 判斷射線是否命中 UI
        bool isGetUIRaycast = _rayInteractor.TryGetCurrentUIRaycastResult(out _);

        // 更新最後一次射線偵測結果
        _placementManager.UpdateXRDrag(ray, isGetUIRaycast);

        _placementManager.EndDrag(false);

        _currentPrefab = null;
        _isDragging = false;
    }
}
