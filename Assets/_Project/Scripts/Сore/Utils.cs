using System;
using UnityEngine;

namespace Game.Utilities
{
    public static class Utils
    {
        public static void UpdateTimer(ref float value, Action callback)
        {
            if (value > 0f)
            {
                value -= Time.deltaTime;
                if (value <= 0f)
                {
                    value = 0f;
                    callback?.Invoke();
                }
            }
        }

        public static void RandomizeValue(ref float value, in Vector2 range)
        {
            value = UnityEngine.Random.Range(range.x, range.y);
        }

        public static void RandomizeValue(ref int value, in Vector2Int range)
        {
            value = UnityEngine.Random.Range(range.x, range.y);
        }

        public static void DestroyAllChildren(this RectTransform rectTransform)
        {
            // Iterate backwards to avoid index shifting issues when destroying
            for (int i = rectTransform.childCount - 1; i >= 0; i--)
            {
                Transform child = rectTransform.GetChild(i);

#if UNITY_EDITOR
                // Use DestroyImmediate in Edit Mode / Prefab editing
                if (!Application.isPlaying)
                    UnityEngine.Object.DestroyImmediate(child.gameObject);
                else
#endif
                    // Use Destroy at runtime (deferred to end of frame)
                    UnityEngine.Object.Destroy(child.gameObject);
            }
        }
    }
}