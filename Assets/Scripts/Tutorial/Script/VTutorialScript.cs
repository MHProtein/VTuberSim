using VTuber.Core.ScriptSystem;
using VTuber.ScheduleSystem.Events;

namespace Tutorial.Script
{
    public class VTutorialScript : VScript
    {
        public VTutorialScriptConfiguration Configuration => _tutorialConfiguration;
        private VTutorialScriptConfiguration _tutorialConfiguration;
        public VTutorialScript(VScriptConfiguration configuration) : base(configuration)
        {
            _tutorialConfiguration = configuration as VTutorialScriptConfiguration;
        }

        public override VScheduleEvent NextWeek()
        {
            var e = base.NextWeek();
            
            return e;
        }
    }
}