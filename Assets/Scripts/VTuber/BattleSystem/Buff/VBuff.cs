using System.Collections.Generic;
using VTuber.BattleSystem.Core.Buff.Actions;

namespace VTuber.BattleSystem.Core.Buff
{
    public class VBuff
    {
        VBuffConfiguration configuration;
        private List<VAction> actions;
        
        public VBuff(VBuffConfiguration configuration)
        {
            this.configuration = configuration;
            actions = new List<VAction>();
            foreach (var actionConfiguration in configuration.actionConfigurations)
            {
                actions.Add(actionConfiguration.CreateAction());
            }
        }
    }
}