using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Character;
using VTuber.Core.ScriptSystem;
using VTuber.Core.StateMachine;
using VTuber.EventSystem;
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
        public VBattleSaveData battleSaveData;
        
        public VScheduleUISaveData scheduleUISaveData;
        public VStoreSaveData storeSaveData;
        public VEventSystemSaveData eventSystemSaveData;
        public TimeSpan lastPlayTime;

        public uint cardIDDistributor;
        public uint consumableIDDistributor;
    }
}