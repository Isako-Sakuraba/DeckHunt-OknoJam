using UnityEngine;

namespace Game.Enemies
{
    public readonly struct RectBounds
    {
        public RectBounds(Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
        }

        public RectBounds(Vector3 topLeft, Vector3 bottomRight)
        {
            TopLeft = topLeft;
            TopRight = new Vector3(bottomRight.x, topLeft.y, topLeft.z);
            BottomLeft = new Vector3(topLeft.x, bottomRight.y, bottomRight.z);
            BottomRight = bottomRight;
        }

        public Vector3 TopLeft { get; }
        public Vector3 TopRight { get; }
        public Vector3 BottomLeft { get; }
        public Vector3 BottomRight { get; }

        public Vector3 Center => (TopLeft + TopRight + BottomLeft + BottomRight) / 4f;

        public float Height => Vector3.Distance(BottomLeft, TopLeft);
        public float Width => Vector3.Distance(TopLeft, TopRight);

        public Vector3 GetPosition(float normalizedWidth, float normalizedHeight)
        {
            // Clamp values to ensure they stay within bounds
            normalizedWidth = Mathf.Clamp01(normalizedWidth);
            normalizedHeight = Mathf.Clamp01(normalizedHeight);

            // Interpolate along the top and bottom edges
            Vector3 topPoint = Vector3.Lerp(TopLeft, TopRight, normalizedWidth);
            Vector3 bottomPoint = Vector3.Lerp(BottomLeft, BottomRight, normalizedWidth);

            // Interpolate between the top and bottom points
            return Vector3.Lerp(topPoint, bottomPoint, normalizedHeight);
        }
    }
}