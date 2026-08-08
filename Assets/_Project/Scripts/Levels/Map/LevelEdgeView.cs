using Alchemy.Inspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels
{
    public class LevelEdgeView : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, Required] private LevelEdge _edge;
        [SerializeField, Required] private Graphic _graphic;
        [SerializeField, Required] private RectTransform _rectTransform;

        public Color BaseColor => _edge.State == LevelEdge.EdgeState.Completed ? Color.green : Color.white;

        private void OnEnable()
        {
            _edge.StateChanged += OnStateChanged;
            _edge.HighlightChanged += OnHighlightChanged;
            _edge.Initialized += OnInitialized;
        }

        private void OnDisable()
        {
            _edge.StateChanged -= OnStateChanged;
            _edge.HighlightChanged -= OnHighlightChanged;
            _edge.Initialized -= OnInitialized;
        }

        private void OnHighlightChanged(bool value)
        {
            if (value)
                _graphic.color = Color.yellow;
            else
                _graphic.color = BaseColor;
        }

        private void OnStateChanged(LevelEdge.EdgeState state)
        {
            UpdateGraphics();
        }

        private void UpdateGraphics()
        {
            _graphic.color = BaseColor;
        }

        private void OnInitialized(LevelNode node1, LevelNode node2)
        {
            if (node1 == null || node2 == null || _rectTransform == null)
                return;

            Transform parent = _rectTransform.parent;
            if (parent == null)
                return;

            Vector2 startLocal = GetNodePointInParentSpace(parent, node1);
            Vector2 endLocal = GetNodePointInParentSpace(parent, node2);

            Vector2 delta = endLocal - startLocal;
            float distance = delta.magnitude;

            if (distance < 0.001f)
                return;

            // Preserve current visual width.
            float width = _rectTransform.rect.width;

            // Make the edge controlled manually by position/rotation/size.
            // This prevents anchors from stretching it unexpectedly.
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            // Bottom-center pivot makes the rect start at node1 and grow toward node2.
            _rectTransform.pivot = new Vector2(0.5f, 0f);

            // Place edge start at first node.
            _rectTransform.localPosition = startLocal;

            // Rotate local "up" axis toward the second node.
            Vector2 direction = delta / distance;

            // Angle needed to rotate Vector2.up into direction.
            float zAngle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
            _rectTransform.localRotation = Quaternion.Euler(0f, 0f, zAngle);

            // Stretch using height only. Width remains the same.
            _rectTransform.sizeDelta = new Vector2(width, distance);

            UpdateGraphics();
        }

        private Vector2 GetNodePointInParentSpace(Transform parent, LevelNode node)
        {
            // This assumes LevelNode is a MonoBehaviour or has a Transform.
            // If your LevelNode exposes a RectTransform/WorldPosition differently,
            // replace this part with that source.
            Transform nodeTransform = node.transform;

            // If the node is UI, use the center of its RectTransform.
            if (nodeTransform is RectTransform nodeRect)
            {
                Vector3 worldCenter = nodeRect.TransformPoint(nodeRect.rect.center);
                return parent.InverseTransformPoint(worldCenter);
            }

            // Fallback for normal Transform-based nodes.
            return parent.InverseTransformPoint(nodeTransform.position);
        }
    }
}