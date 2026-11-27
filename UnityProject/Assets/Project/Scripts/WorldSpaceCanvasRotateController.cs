using UnityEngine;

public class WorldSpaceCanvasRotateController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera _xrCamera;

    private void LateUpdate()
    {
        if (_xrCamera == null)
        {
            Debug.LogWarning("XR Camera ©|¥¼«ü©w¡I", this);
            return;
        }

        transform.rotation = _xrCamera.transform.rotation;
    }
}