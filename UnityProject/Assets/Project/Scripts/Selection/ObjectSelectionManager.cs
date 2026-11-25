
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ObjectSelectionManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera _mainCamera;

    [Header("Selection")]
    [SerializeField] private LayerMask _selectableLayer;
    [SerializeField] private float _maxRayDistance = 500f;

    [Header("Interaction Menu")]
    [SerializeField] private GameObject _interactionMenu; // 拖入想指定的特定 UI（例如特定 Panel 或 Button）
    [SerializeField] private Vector3 _menuOffset = new Vector3(0f, 1.5f, 0f);

    private readonly List<RaycastResult> _raycastResults = new();

    private SelectionHighlight _currentHighlight;
    private GameObject _selectedObject;

    public GameObject SelectedObject => _selectedObject;

    private void Start()
    {
        _interactionMenu.SetActive(false);
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if(!IsPointerOverSpecificUi(mousePosition, _interactionMenu))
                SelectObject(mousePosition);
        }
    }

    private void SelectObject(Vector2 mousePosition)
    {
        Ray ray = _mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, _maxRayDistance, _selectableLayer))
        {
            ClearSelection();

            _selectedObject = hit.collider.gameObject;

            _currentHighlight = _selectedObject.GetComponent<SelectionHighlight>();

            if (_currentHighlight != null)
                _currentHighlight.SetHighlight(true);

            ShowInteractionMenu();
        }
        else
        {
            ClearSelection();
        }
    }

    private void ShowInteractionMenu()
    {
        Vector3 worldPosition = _selectedObject.transform.position + _menuOffset;

        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);

        _interactionMenu.transform.position = screenPosition;
        _interactionMenu.SetActive(true);
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
