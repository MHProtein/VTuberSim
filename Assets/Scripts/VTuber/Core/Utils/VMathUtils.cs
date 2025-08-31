using System;
using UnityEngine;

namespace VTuber.Core.UI
{
    public static class VMathUtils
    {
        public enum RoundingType
        {
            Ceil,
            Floor,
            Round
        }
        public static int FloatToInt(float value, RoundingType type = RoundingType.Ceil)
        {
            switch (type)
            {
                case RoundingType.Ceil:
                    return Mathf.CeilToInt(value);
                case RoundingType.Floor:
                    return Mathf.FloorToInt(value);
                case RoundingType.Round:
                    return Mathf.RoundToInt(value);
            }
            return 0;
        }
    }
}