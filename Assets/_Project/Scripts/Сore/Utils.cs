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
    }
}