using System;
using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.ScriptSystem;
using VTuber.Core.StateMachine;
using VTuber.Reincarnation;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.UI;
using VTuber.Store;

namespace SlayTheSpire.System.SavingSystem
{
    public class SaveData
    {
        public List<VAccountSaveData> accounts;
        public VCharacterSaveData characterSaveData;
        public VStateMachineSaveData stateMachine;
        public VWeeklyScheduleSaveData weeklySchedule;
        public VScriptSaveData script;
        
        public VScheduleUISaveData scheduleUISaveData;
        public VStoreSaveData storeSaveData;
        public TimeSpan lastPlayTime;
    }
}