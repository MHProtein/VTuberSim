using UnityEngine;

namespace VTuber.Core.UI
{
    public enum VOperatorType
    {
        LessThan,
        LessEqual,
        Equal,
        GreaterThan,
        GreaterEqual,
        NotEqual
    }

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

        public static string GetPercentage(int value, int decimalPlaces)
        {
            var v = value - 100f;
            return v.ToString();
        }

        public static bool Compare(int value, int targetValue, VOperatorType operatorType)
        {
            switch (operatorType)
            {
                case VOperatorType.LessThan:
                    return value < targetValue;
                case VOperatorType.LessEqual:
                    return value <= targetValue;
                case VOperatorType.Equal:
                    return value == targetValue;
                case VOperatorType.GreaterThan:
                    return value > targetValue;
                case VOperatorType.GreaterEqual:
                    return value >= targetValue;
                case VOperatorType.NotEqual:
                    return value != targetValue;
            }

            return false;
        }
    }
}