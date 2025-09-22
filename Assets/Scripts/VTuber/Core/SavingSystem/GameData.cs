using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.ScriptSystem;
using VTuber.Core.StateMachine;
using VTuber.Reincarnation;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.UI;

namespace SlayTheSpire.System.SavingSystem
{
    public class GameData
    {
        public List<VAccountSaveData> accounts;
        public VCharacterSaveData characterSaveData;
        public VStateMachineSaveData stateMachine;
        public VWeeklyScheduleSaveData weeklySchedule;
        public VScriptSaveData script;
        
        public VScheduleUISaveData scheduleUISaveData;
    }
}