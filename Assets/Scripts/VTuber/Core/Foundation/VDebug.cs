using UnityEngine;

namespace VTuber.Core.Foundation
{
    public static class VDebug
    {
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
        
        private static bool _isDebugEnabled = true;
        
        public static void Log(object message, bool ignore = false)
        {
            if (!ignore && !IsDebugEnabled) return;
            Debug.Log(message);
        }
        
        public static void LogWarning(object message, bool ignore = false)
        {
            if (!ignore && !IsDebugEnabled) return;
            Debug.LogWarning(message);
        }
        
        public static void LogError(object message, bool ignore = false)
        {
            if (!ignore && !IsDebugEnabled) return;
            Debug.LogError(message);
        }
        
    }
}