using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.UI;

/// <summary>
/// 負責偵測目前是否啟用 XR 裝置，並管理操作模式。
/// </summary>
public class DeviceModeManager : MonoBehaviour
{
    [SerializeField] private GameObject _xrOrigin;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private InputSystemUIInputModule _inputSystemUIInputModule;
    [SerializeField] private XRUIInputModule _xrUIInputModule;
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private Canvas _VRPropListCanvas;

    private bool _isXRMode = false;
    public bool IsXRMode { get => _isXRMode; }

    //單例模式（Singleton）
    public static DeviceModeManager Instance { get; private set; }

    private void Awake()
    {
        //檢查是否已實例此物件，是:摧毀目前物件，否:靜態記憶體指向此物件
        if (Instance != null)
        {
            Debug.Log("More than one DeviceModeManager active in the scene! Destroying lastest one: " + name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _isXRMode = XRSettings.isDeviceActive;

        if (_isXRMode)
        {
            _xrOrigin.gameObject.SetActive(true);
            _mainCamera.gameObject.SetActive(false);
            _inputSystemUIInputModule.enabled = false;
            _xrUIInputModule.enabled = true;
            _mainCanvas.gameObject.SetActive(false);
            _VRPropListCanvas.gameObject.SetActive(true);
        }
        else
        {
            _xrOrigin.gameObject.SetActive(false);
            _mainCamera.gameObject.SetActive(true);
            _inputSystemUIInputModule.enabled = true;
            _xrUIInputModule.enabled = false;
            _mainCanvas.gameObject.SetActive(true);
            _VRPropListCanvas.gameObject.SetActive(false);
        }
    }
}