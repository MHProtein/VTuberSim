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
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetBattleUIScale(0.75f);
        }
        
        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetBattleUIScale(1.0f);
        }
    }
}