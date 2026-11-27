
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static UnityEngine.GraphicsBuffer;

public class ObjectSelectionManager : MonoBehaviour
{
    [Header("XR Settings")]
    [SerializeField] private InputActionReference _xrSelectAction;
    [SerializeField] private Transform _xrRayOrigin;
    [SerializeField] private XRRayInteractor _xrRayInteractor;

    [Header("Camera")]
    [SerializeField] private Camera _mainCamera;

    [Header("Selection")]
    [SerializeField] private LayerMask _selectableLayer;
    [SerializeField] private float _maxRayDistance = 500f;

    [Header("Interaction Menu")]
    [SerializeField] private GameObject _interactionMenu; // 拖入想指定的特定 UI（例如特定 Panel 或 Button）
    [SerializeField] private Vector3 _menuOffset = new Vector3(0f, 1.5f, 0f);

    [Header("VR Interaction Menu")]
    [SerializeField] private GameObject _vrInteractionMenu;
    [SerializeField] private Vector3 _vrMenuOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private Transform _worldSpaceCanvas;

    private readonly List<RaycastResult> _raycastResults = new();

    private SelectionHilight _currentHighlight;
    private GameObject _selectedObject;

    public GameObject SelectedObject => _selectedObject;

    private DeviceModeManager _deviceModeManager;

    //單例模式（Singleton）
    public static ObjectSelectionManager Instance { get; private set; }

    private void Awake()
    {
        //檢查是否已實例此物件，是:摧毀目前物件，否:靜態記憶體指向此物件
        if (Instance != null)
        {
            Debug.Log("More than one ObjectSelectionManager active in the scene! Destroying lastest one: " + name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        _deviceModeManager = DeviceModeManager.Instance;
        _interactionMenu.SetActive(false);
        _vrInteractionMenu.SetActive(false);
    }

    private void Update()
    {
        if (_deviceModeManager.IsXRMode)
        {
            if (_xrSelectAction != null && _xrSelectAction.action.WasPressedThisFrame())
            {
                // 如果 XR 射線目前命中 UI，就不清除選取
                if (_xrRayInteractor != null &&
                    _xrRayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult uiResult) &&
                    uiResult.gameObject != null)
                {
                    // 若命中物件就是目標 UI，或是目標 UI 的子物件（例如按鈕上的文字或圖示）
                    if (uiResult.gameObject == _vrInteractionMenu ||
                        uiResult.gameObject.transform.IsChildOf(_vrInteractionMenu.transform))
                    {
                        return;
                    }
                }

                // 如果沒有命中可選取的 3D 物件，才清除選取
                if (!Physics.Raycast(_xrRayOrigin.position, _xrRayOrigin.forward, _maxRayDistance, _selectableLayer))
                {
                    ClearSelection();
                }
            }

            return;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (!IsPointerOverSpecificUi(mousePosition, _interactionMenu))
                SelectObject(mousePosition);
        }
    }

    private void SelectObject(Vector2 mousePosition)
    {
        Ray ray = _mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit,
            _maxRayDistance, _selectableLayer))
        {
            SetSelectedObject(hit.collider.gameObject);
        }
        else
        {
            ClearSelection();
        }
    }

    private void SetSelectedObject(GameObject target)
    {
        ClearSelection();

        if (target == null)
            return;

        _selectedObject = target;

        _currentHighlight = _selectedObject.GetComponent<SelectionHilight>();

        if (_currentHighlight != null)
            _currentHighlight.SetHighlight(true);

        ShowInteractionMenu();
    }

    //物件上的XR Simple Interactable的Interactable Events中的Select Entered事件呼叫
    public void SelectObjectFromXR(SelectEnterEventArgs args)
    {
        if (args.interactableObject == null)
            return;

        GameObject target = args.interactableObject.transform.gameObject;
        
        SetSelectedObject(target);
    }

    private void ShowInteractionMenu()
    {
        if (!_deviceModeManager.IsXRMode)
        {
            Vector3 worldPosition = _selectedObject.transform.position + _menuOffset;

            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);

            _interactionMenu.transform.position = screenPosition;
            _interactionMenu.SetActive(true);
        }
        else
        {
            Collider targetCollider = _selectedObject.GetComponent<Collider>();

            Vector3 menuPosition = _selectedObject.transform.position;

            if (targetCollider != null)
            {
                menuPosition = targetCollider.bounds.center;
                menuPosition.y = targetCollider.bounds.max.y;
            }

            menuPosition.y += _vrMenuOffset.y;

            _worldSpaceCanvas.position = menuPosition;

            _vrInteractionMenu.SetActive(true);
        }
    }

    private void ClearSelection()
    {
        if (_currentHighlight != null)
        {
            _currentHighlight.SetHighlight(false);
        }

        _currentHighlight = null;
        _selectedObject = null;
        _interactionMenu.SetActive(false);
        _vrInteractionMenu.SetActive(false);
    }

    /// <summary>
    /// 刪除目前選取的場景物件。
    /// </summary>
    public void DeleteSelectedObject()
    {
        if (_selectedObject == null)
            return;

        Destroy(_selectedObject);

        ClearSelection();

    }

    /// <summary>
    //用 EventSystem.current.RaycastAll(...) 取得目前指標位置命中的所有 UI Raycast 結果，再比對結果裡是否包含目標 UI。
    /// </summary>
    public bool IsPointerOverSpecificUi(Vector2 screenPosition, GameObject targetUiObject)
    {
        if (EventSystem.current == null || targetUiObject == null) return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        _raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerData, _raycastResults);

        foreach (RaycastResult result in _raycastResults)
        {
            // 若命中物件就是目標 UI，或是目標 UI 的子物件（例如按鈕上的文字或圖示）
            if (result.gameObject == targetUiObject || result.gameObject.transform.IsChildOf(targetUiObject.transform))
            {
                return true;
            }
        }

        return false;
    }
}
