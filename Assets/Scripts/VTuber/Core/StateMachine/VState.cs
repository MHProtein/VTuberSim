using System.Collections.Generic;
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
        None
    }

    public class VStateSaveData
    {
        public List<VScheduleEventSaveData> dayEndEvents;
        public bool isLastStreamSuccess;
        public int lastStreamPopularity;
        public bool shouldEndGame;

        // Execution state
        public bool shouldSwitchToModifySchedule;
        public VStateType stateType;
    }

    public abstract class VState
    {
        protected VStateMachine stateMachine;

        protected VStateType stateType;
        public VStateType StateType => stateType;

        public virtual void Register(VStateMachine vStateMachine)
        {
            stateMachine = vStateMachine;
        }

        public virtual void Unregister()
        {
        }

        public virtual void Enter(VState state, params object[] enterParams)
        {
        }

        public virtual void Exit(VState nextState)
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void LateUpdate()
        {
        }

        public virtual VStateSaveData Save()
        {
            return new VStateSaveData { stateType = stateType };
        }

        public virtual void Load(VStateSaveData saveData)
        {
            stateType = saveData.stateType;
        }
    }
}