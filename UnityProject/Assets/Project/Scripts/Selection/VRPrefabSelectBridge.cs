using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // 視 XRIT 版本而定

public class VRPrefabSelectBridge : MonoBehaviour
{
    private IXRSelectInteractable interactable;
    private ObjectSelectionManager _objectSelectionManager;

    private void Awake()
    {
        interactable = GetComponent<IXRSelectInteractable>();
        // 尋找場景中的目標物件腳本（也可改用單例 / 管理器取得）
        _objectSelectionManager = FindFirstObjectByType<ObjectSelectionManager>();
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (_objectSelectionManager != null)
        {
            _objectSelectionManager.SelectObjectFromXR(args);
        }
    }
}