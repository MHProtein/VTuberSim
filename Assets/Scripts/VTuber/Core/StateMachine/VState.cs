using System.Collections.Generic;
using UnityEngine;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Core.StateMachine
{
    public enum VStateType
    {
        ScheduleCreation,
        Execution,
        Pause,
        ScheduleModify,
        PhaseStart,
        None,
    }

    public class VStateSaveData
    {
        public VStateType stateType;
        
        // Execution state
        public bool shouldSwitchToModifySchedule;
        public List<VScheduleEventSaveData> dayEndEvents;
        public bool shouldEndGame;
        public int lastStreamPopularity;
        public bool isLastStreamSuccess;
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
        
        public virtual VStateSaveData Save() { return new VStateSaveData { stateType = stateType }; }

        public virtual void Load(VStateSaveData saveData)
        {
            stateType = saveData.stateType;
        }
    }
}