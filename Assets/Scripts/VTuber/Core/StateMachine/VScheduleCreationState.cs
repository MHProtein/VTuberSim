using System.Collections.Generic;
using SlayTheSpire.System.SavingSystem;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.SE;
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

            if(stateMachine.isTutorial)
                VRaisingUI.Instance.SetTips(stateMachine.TutorialScript.CurrentWeekTip);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetScheduleUIPositionToCreation().OnComplete(() =>
            {
                stateMachine.ScheduleUI.SwitchToCreation(stateMachine.Character, stateMachine.Script,
                    stateMachine.Script.WeekIndex);
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSwitchToScheduleCreation,
                    new Dictionary<string, object>());
                VDataPersistenceManager.Instance.SaveGame();
                VDataPersistenceManager.Instance.SaveGameTutorialWeek();
                VAudioPlayer.Instance.PlayBGM(VBGMType.ScheduleCreation);
            });
        }

        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetCreationUIActive(false);
            VAudioPlayer.Instance.StopBGM();
        }
    }
}