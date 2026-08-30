using UnityEngine;

namespace Game.Enemies
{
    public readonly struct VerticalBounds
    {
        public Vector3 Bottom { get; }
        public Vector3 Top { get; }

        public Vector3 Center => (Bottom + Top) * 0.5f;
        public float Height => Vector3.Distance(Bottom, Top);

        public VerticalBounds(Vector3 bottom, Vector3 top)
        {
            Bottom = bottom;
            Top = top;
        }

        public Vector3 GetPosition(float normalizedHeight)
        {
            return Vector3.Lerp(Bottom, Top, normalizedHeight);
        }
    }
}