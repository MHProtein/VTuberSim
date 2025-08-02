using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Buff
{
    public class VRelic
    {
        private VRelicConfiguration _configuration;
        public List<VEffect> Effects { get; }
        public uint ConfigId => _configuration.id;
        public string GetRelicName() => _configuration.relicName;

        public VRelic(VRelicConfiguration config, List<VEffect> effects)
        {
            _configuration = config;
            Effects = effects;
        }

        public void Activate(VBattle battle)
        {
            foreach (var effect in Effects)
            {
                effect.OnRelicActivated(battle);
            }
        }

        public void OnRelicRemoved()
        {
            foreach (var effect in Effects)
            {
                effect.OnRelicRemoved();
            }
        }
    }
