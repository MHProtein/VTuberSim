using System;

namespace VTuber.Core.Foundation
{
    public class VSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateInstance<T>();
                }
                return _instance;
            }
        }

        private static T1 CreateInstance<T1>()
        {
            return Activator.CreateInstance<T1>();
        }

        ~VSingleton()
        {
            if (_instance != null)
            {
                _instance = default(T);
            }
        }
        
    }
}