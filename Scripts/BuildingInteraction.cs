// BuildingInteraction.cs
// --------------------------------------------------
// Handles tap interaction on a building prefab and shows a tooltip.
// --------------------------------------------------
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class BuildingInteraction : MonoBehaviour {
    public string buildingName = "Building";
    public string description = "Description of the building.";
    private GameObject tooltip;

    private void Start() {
        // Simple tooltip UI: a world-space canvas with TextMeshPro (fallback to Text).
        tooltip = new GameObject("Tooltip");
        tooltip.transform.SetParent(this.transform);
        var text = tooltip.AddComponent<UnityEngine.UI.Text>();
        var canvas = tooltip.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var rect = tooltip.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);
        text.text = $"{buildingName}\n{description}";
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        tooltip.SetActive(false);
    }

    private void OnMouseDown() {
        // Toggle tooltip visibility on tap.
        tooltip.SetActive(!tooltip.activeSelf);
        // Auto‑hide after 3 seconds.
        if (tooltip.activeSelf) {
            Invoke(nameof(HideTooltip), 3f);
        }
    }

    private void HideTooltip() {
        tooltip.SetActive(false);
    }
}
