using Sirenix.OdinInspector;

namespace VTuber.Core.Foundation
{
    public class VMonoBehaviour : SerializedMonoBehaviour
    {
        protected virtual void Awake()
        {
        }

        protected virtual void Start()
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