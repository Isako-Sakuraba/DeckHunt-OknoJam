using Alchemy.Inspector;
using LitMotion;
using LitMotion.Extensions;
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

        [Header("Settings")]
        [SerializeField] private Color _baseColor = Color.brown;
        [SerializeField] private Color _visitedColor = Color.darkGreen;
        [SerializeField] private Color _highlightedColor = Color.orange;
        [SerializeField] private float _nodeOffset = 50f;
        [SerializeField] private float _colorChangeDuration = 0.15f;

        private bool _highlighted = false;

        private MotionHandle _colorMotion;

        private void OnEnable()
        {
            _edge.StateChanged += OnStateChanged;
            _edge.HighlightChanged += OnHighlightChanged;
            _edge.Initialized += OnInitialized;

            UpdateGraphicsImmediately();
        }

        private void OnDisable()
        {
            _edge.StateChanged -= OnStateChanged;
            _edge.HighlightChanged -= OnHighlightChanged;
            _edge.Initialized -= OnInitialized;
        }

        private void OnHighlightChanged(bool value)
        {
            _highlighted = value;
            UpdateGraphics();
        }

        private void OnStateChanged(LevelEdge.EdgeState state)
        {
            UpdateGraphics();
        }

        private void UpdateGraphics()
        {
            var targetColor = GetTargetColor();

            if (_graphic.color != targetColor)
            {
                if (_colorMotion.IsValid() && _colorMotion.IsPlaying())
                    _colorMotion.Cancel();

                _colorMotion = LMotion.Create(_graphic.color, targetColor, _colorChangeDuration)
                    .BindToColor(_graphic);
            }
        }

        private void UpdateGraphicsImmediately()
        {
            var targetColor = GetTargetColor();
            _graphic.color = targetColor;
        }

        public Color GetTargetColor()
        {
            var targetColor = _highlighted ? _highlightedColor : _baseColor;
            targetColor = _edge.State == LevelEdge.EdgeState.Completed ? _visitedColor : targetColor;
            return targetColor;
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

            // Don't draw if nodes are too close or overlap within the offset
            float effectiveDistance = distance - 2f * _nodeOffset;
            if (effectiveDistance < 0.001f)
            {
                _graphic.enabled = false;
                return;
            }

            _graphic.enabled = true;

            // Preserve current visual width
            float width = _rectTransform.rect.width;

            // Manual control: no anchor stretching
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            // Bottom-center pivot so height grows upward along local Y
            _rectTransform.pivot = new Vector2(0.5f, 0f);

            // Calculate normalized direction
            Vector2 direction = delta / distance;

            // This centers the edge in the gap between the two offset boundaries
            _rectTransform.localPosition = startLocal + direction * _nodeOffset;

            // Rotate local "up" axis toward node2
            float zAngle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
            _rectTransform.localRotation = Quaternion.Euler(0f, 0f, zAngle);

            // Height uses the effective distance (already offset from both sides)
            _rectTransform.sizeDelta = new Vector2(width, effectiveDistance);

            UpdateGraphicsImmediately();
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