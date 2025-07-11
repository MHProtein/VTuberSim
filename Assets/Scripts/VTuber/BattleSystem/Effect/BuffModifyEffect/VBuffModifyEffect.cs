using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VBuffModifyEffect : VEffect
    {
  
        VBuffModifyEffectConfiguration _configuration;
        public VBuffModifyEffect(VBuffModifyEffectConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public override void ApplyEffect(VBattle battle)
        {
            if(battle.BuffManager.TryGetBuff(_configuration.buffName, out var buff))
            {
                buff.AddLayerOrDuration(_configuration.addValue);
                VDebug.Log($"Effect {_configuration.effectName} applied {_configuration.addValue} to buff {buff.GetBuffName()} with ID {buff.Id}");
            }
        }
            
    }
}