
using UnityEngine;

public class SelectionHilight : MonoBehaviour
{
    [Header("Highlight Settings")]
    [SerializeField] private Color highlightColor = Color.yellow;

    private Renderer[] renderers;
    private Color[] originalColors;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    /// <summary>
    /// 設定物件是否顯示選取顏色。
    /// </summary>
    public void SetHighlight(bool enabled)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color =
                enabled ? highlightColor : originalColors[i];
        }
    }
}
