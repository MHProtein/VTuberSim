using System;
using System.Collections.Generic;
using NUnit.Framework;
using VTuber.Core.ScriptSystem;
using VTuber.ScheduleSystem.Events;

namespace Tutorial.Script
{
    public class VTutorialScript : VScript
    {
        public List<uint> CurrentWeekEventList => Configuration.weeks[_weekIndex].eventIDs;
        public List<VTutorialWeekCondition> CurrentWeekConditions => Configuration.weeks[_weekIndex].conditions;
        public VTutorialScriptConfiguration Configuration => _tutorialConfiguration;
        private VTutorialScriptConfiguration _tutorialConfiguration;
        private Action<int> _onWeekAdvanced;
        public VTutorialScript(VScriptConfiguration configuration) : base(configuration)
        {
            _tutorialConfiguration = configuration as VTutorialScriptConfiguration;
        }
        
        public void AddOnWeekAdvancedCallback(Action<int> callback)
        {
            _onWeekAdvanced += callback;
        }

        public override VScheduleEvent NextWeek()
        {
            var e = base.NextWeek();
            _onWeekAdvanced?.Invoke(_weekIndex);
            return e;
        }
    }
}