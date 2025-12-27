using System.Collections.Generic;
using SlayTheSpire.System.SavingSystem;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.SE;
using VTuber.Dialogue.UI;
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
            VEventSystemUI.Instance.CloseEventUI();
            VSingletonMonobehaviour<VEventSystemUI>.Instance.SetFullScreenButtonActive(false);
            stateMachine.ScheduleUI.SwitchToCreation(stateMachine.Character, stateMachine.Script,
                stateMachine.Script.WeekIndex);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetScheduleUIPositionToCreation().OnComplete(() =>
            {
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSwitchToScheduleCreation,
                    new Dictionary<string, object>());
                VDataPersistenceManager.Instance.SaveGame(VSavePointType.ScheduleCreation);
                VDataPersistenceManager.Instance.SaveGameTutorialWeek();
                VAudioPlayer.Instance.PlayBGM(VBGMType.ScheduleCreation);
            });
        }

        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetCreationUIActive(false);
            VSingletonMonobehaviour<VEventSystemUI>.Instance.SetFullScreenButtonActive(true);
            VAudioPlayer.Instance.StopBGM();
        }
    }
}