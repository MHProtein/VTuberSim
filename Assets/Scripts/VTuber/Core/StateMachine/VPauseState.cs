using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.StateMachine
{
    public class VPauseState : VState
    {
        public VPauseState()
        {
            stateType = VStateType.Pause;
        }

        public override void Enter(VState state, params object[] enterParams)
        {
            base.Enter(state, enterParams);
            if(state.StateType == VStateType.Execution)
                VSingletonMonobehaviour<VRaisingUI>.Instance.SetBattlePause(true);
        }
        
        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            if(nextState.StateType == VStateType.Execution)
                VSingletonMonobehaviour<VRaisingUI>.Instance.SetBattlePause(false);
        }
    }
}