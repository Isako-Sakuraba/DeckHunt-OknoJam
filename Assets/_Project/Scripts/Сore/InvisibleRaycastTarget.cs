using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InvisibleRaycastTarget : Graphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Intentionally empty — no vertices, no draw calls
        vh.Clear();
    }
}