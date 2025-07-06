namespace VTuber.BattleSystem.Core.Buff.Actions
{
    public class VAction
    {
        public VActionConfiguration configuration;

        public VAction(VActionConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        public virtual void ApplyAction()
        {
        }
    }
}