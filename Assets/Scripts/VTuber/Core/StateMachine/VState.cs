using UnityEngine;

namespace VTuber.Core.StateMachine
{
    public abstract class VState
    {
        public string StateName { get => stateName; set => stateName = value; }
        public bool ViewChangable = false;
        
        [SerializeField] protected string stateName;
        
        protected VStateMachine stateMachine;
        
        public virtual void Register(VStateMachine vStateMachine)
        {
            this.stateMachine = vStateMachine;
        }
        
        public virtual void Unregister() { }
        
        public virtual void Enter(params object[] enterParams) { }

        public virtual void Exit(VState nextState)
        {
            
        }
        
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void LateUpdate() { }
    }
}