using UnityEngine;

namespace VTuber.Core.Foundation
{
    public class VMonoBehaviour : MonoBehaviour
    {
        protected void Awake()
        {
        
        }

        protected void Start()
        {
    
        }

        private void Update()
        {
            OnUpdateBegin();
            UpdateImpl();
            OnUpdateEnd();
        }

        protected virtual void OnEnable()
        {
        
        }
    
        protected virtual void OnDisable()
        {
        
        }
    
        protected virtual void OnUpdateBegin()
        {
        
        }
    
        protected virtual void OnUpdateEnd()
        {
        
        }

        protected virtual void UpdateImpl()
        {
        
        }
    }
}

