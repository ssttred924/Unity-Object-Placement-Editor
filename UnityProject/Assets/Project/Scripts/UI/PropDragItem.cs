
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 負責處理 UI 道具按鈕的拖曳操作。
/// 透過 Unity EventSystem 接收開始拖曳、持續拖曳
/// 與結束拖曳事件，並通知 PlacementManager
/// 更新物件預覽或執行放置、取消放置。
/// </summary>
//當 Unity 的 EventSystem 偵測到符合條件的 UI 拖曳事件時，就會尋找實作了 IBeginDragHandler 的元件，並呼叫它的 OnBeginDrag()。
public class PropDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]

    [SerializeField] private GameObject _propPrefab;

    private PlacementManager _placementManager;
    private DeviceModeManager _deviceModeManager;

    private void Start()
    {
        _placementManager = PlacementManager.Instance;
        _deviceModeManager = DeviceModeManager.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_placementManager == null || _deviceModeManager == null || _propPrefab == null)
        {
            Debug.LogWarning("PlacementManager or DeviceModeManager or Prefab is missing.");
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left || _deviceModeManager.IsXRMode)
            return;

        _placementManager.BeginDrag(_propPrefab, eventData.position);

        _placementManager.UpdateDrag(eventData.position, eventData.pointerId);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_placementManager == null || _deviceModeManager == null)
        {
            Debug.LogWarning("PlacementManager or DeviceModeManager is missing.");
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left || _deviceModeManager.IsXRMode)
            return;;

        _placementManager.UpdateDrag(eventData.position, eventData.pointerId);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_placementManager == null || _deviceModeManager == null)
        {
            Debug.LogWarning("PlacementManager or DeviceModeManager is missing.");
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left || _deviceModeManager.IsXRMode)
            return;

        // 更新最後一次滑鼠位置
        _placementManager.UpdateDrag(eventData.position, eventData.pointerId);


        // 放開滑鼠時若位於 UI 上，取消放置
        bool pointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(eventData.pointerId);

        _placementManager.EndDrag(pointerOverUI);
    }

    public GameObject GetPrefab()
    {
        return _propPrefab;
    }
}
