
using UnityEngine;

/// <summary>
/// 控制放置物件的選取提示效果。
/// </summary>
public class SelectionHighlight : MonoBehaviour
{
    [SerializeField] private GameObject _highlightObject;

    private void Awake()
    {
        SetHighlight(false);
    }

    public void SetHighlight(bool enabled)
    {
        if (_highlightObject != null)
        {
            _highlightObject.SetActive(enabled);
        }
    }
}
