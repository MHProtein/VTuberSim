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
        public VBattleSaveData battleSaveData;

        public uint cardIDDistributor;
        public VCharacterSaveData characterSaveData;
        public uint consumableIDDistributor;
        public VEventSystemSaveData eventSystemSaveData;
        public TimeSpan lastPlayTime;

        public VScheduleUISaveData scheduleUISaveData;
        public VScriptSaveData script;
        public VStateMachineSaveData stateMachine;
        public VStoreSaveData storeSaveData;
        public VWeeklyScheduleSaveData weeklySchedule;
        public bool saved;
    }
}