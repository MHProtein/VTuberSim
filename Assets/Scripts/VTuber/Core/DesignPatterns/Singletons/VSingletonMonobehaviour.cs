using System;
using UnityEngine;

namespace VTuber.Core.Foundation
{
    public class VSingletonMonobehaviour<T> : VMonoBehaviour where T : VMonoBehaviour
    {
        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        protected static T instance;

        protected override void Awake()
        {
            if(instance is null)
                instance = this as T;
        }

        protected override void Start()
        {
            
        }

        protected static void CreateInstance()
        {
            if(instance is not null)
                return;

            GameObject go = new GameObject();
            instance = go.AddComponent<T>();
            go.name = instance.GetType().Name;
        }
        
    }
}