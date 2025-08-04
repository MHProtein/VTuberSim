using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.StateMachine
{
    public class VScheduleCreationState : VState
    {
        public VScheduleCreationState()
        {
            stateType = VStateType.ScheduleCreation;
        }
        
        public override void Enter(VState state, params object[] enterParams)
        {
            base.Enter(state, enterParams);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetCreationUIActive(true);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetScheduleUIPositionToCreation().OnComplete((() =>
            {
                
                stateMachine.ScheduleUI.SwitchToCreation(stateMachine.Script, stateMachine.WeekIndex);
            }));
        }

        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetCreationUIActive(false);
        }
    }
}