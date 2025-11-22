using System;
using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.ScriptSystem;
using VTuber.ScheduleSystem.Events;

namespace Tutorial.Script
{
    public class VTutorialScript : VScript
    {
        private Action<int> _onWeekAdvanced;

        public VTutorialScript(VScriptConfiguration configuration) : base(configuration)
        {
            Configuration = configuration as VTutorialScriptConfiguration;
        }

        public List<uint> CurrentWeekDialogEventList => Configuration.weeks[_weekIndex].eventIDs;
        public List<uint> CurrentWeekStreamEventList => Configuration.weeks[_weekIndex].streamEventIDs;
        public List<VTutorialWeekCondition> CurrentWeekConditions => Configuration.weeks[_weekIndex].conditions;
        public VTipConfig CurrentWeekTip => Configuration.weeks[_weekIndex].tip;
        public bool CurrentWeekUseCoopEvent => Configuration.weeks[_weekIndex].useCoopEvents;
        public VTutorialScriptConfiguration Configuration { get; }

        public void AddOnWeekAdvancedCallback(Action<int> callback)
        {
            _onWeekAdvanced += callback;
        }

        public static VTutorialScript Load(VScriptSaveData data, VScriptConfiguration configuration)
        {
            var script = new VTutorialScript(configuration);
            script.currentPhase = script.Phases[data.currentPhaseIndex];
            script._weekIndex = data.weekIndex;
            return script;
        }
        
        public override VScheduleEvent NextWeek()
        {
            var e = base.NextWeek();
            _onWeekAdvanced?.Invoke(_weekIndex);
            return e;
        }

        public bool CheckCurrentWeekConditions(VCharacter character)
        {
            foreach (var condition in CurrentWeekConditions)
                if (!condition.IsTrue(character))
                    return false;
            return true;
        }
    }
}