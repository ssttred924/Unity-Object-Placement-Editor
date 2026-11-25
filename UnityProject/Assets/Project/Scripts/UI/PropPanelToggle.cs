using UnityEngine;
using UnityEngine.UI;

public class PropPanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject _propList;

    private Image _imageBackGround;

    private void Start()
    {
        _imageBackGround = GetComponent<Image>();
    }

    public void Toggle()
    {
        _propList.SetActive(!_propList.activeSelf);
        _imageBackGround.enabled = !_imageBackGround.enabled;
    }
}