using UnityEngine;

namespace VTuber.Core.Foundation
{
    public static class VDebug
    {
        private static bool _isDebugEnabled = true;

        public static bool IsDebugEnabled
        {
            get => _isDebugEnabled;
            set
            {
                if (_isDebugEnabled == value) return;
                _isDebugEnabled = value;
                Debug.Log(_isDebugEnabled ? "Debugging is enabled." : "Debugging is disabled.");
            }
        }

        public static void Log(object message)
        {
            if (!IsDebugEnabled) return;
            Debug.Log(message);
        }

        public static void LogWarning(object message)
        {
            if (!IsDebugEnabled) return;
            Debug.LogWarning(message);
        }

        public static void LogError(object message)
        {
            if (!IsDebugEnabled) return;
            Debug.LogError(message);
        }
    }
}