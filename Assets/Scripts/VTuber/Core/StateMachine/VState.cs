using UnityEngine;

namespace VTuber.Core.StateMachine
{
    public enum VStateType
    {
        ScheduleCreation,
        Execution,
        Pause,
        ScheduleModify
    }
    public abstract class VState
    {
        public VStateType StateType => stateType;
        
        protected VStateType stateType;
        
        protected VStateMachine stateMachine;
        
        public virtual void Register(VStateMachine vStateMachine)
        {
            this.stateMachine = vStateMachine;
        }
        
        public virtual void Unregister() { }
        
        public virtual void Enter(VState state, params object[] enterParams) { }
        
        public virtual void Exit(VState nextState)
        {
            
        }
        
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void LateUpdate() { }
    }
}