using UnityEngine;

namespace VTuber.Core.Foundation
{
    public class VSingletonMonobehaviour<T> : VMonoBehaviour where T : VMonoBehaviour
    {
        protected static T instance;
        public static T Instance => instance;

        protected override void Awake()
        {
            if (instance is null)
                instance = this as T;
        }

        protected override void Start()
        {
        }

        protected static void CreateInstance()
        {
            if (instance is not null)
                return;

            var go = new GameObject();
            instance = go.AddComponent<T>();
            go.name = instance.GetType().Name;
        }
    }
}