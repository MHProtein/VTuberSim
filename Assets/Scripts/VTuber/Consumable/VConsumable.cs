using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.RaisingEffect;

namespace VTuber.Consumable
{
    public class VRaisingConsumable : VConsumable
    {
        public List<VRaisingEffect> raisingEffects;
        public VRaisingConsumable(VConsumableConfiguration configuration) : base(configuration)
        {
            type = VConsumableType.Raising;
        }
        
        public void ApplyEffect(VCharacter character)
        {
            raisingEffects.ForEach(effect => effect.ApplyEffect(character));
        }
    }
    public class VBattleConsumable : VConsumable
    {
        public List<VEffect> effects;
        public VBattleConsumable(VConsumableConfiguration configuration) : base(configuration)
        {
            type = VConsumableType.Stream;
        }

        public void ApplyEffect(VBattle battle)
        {
            effects.ForEach(effect => effect.ApplyEffect(battle, 1, false, false));
        }
        
    }
    public class VConsumable
    {
        public uint Id => _configuration.id;
        public string Name => _configuration.name;
        public string Description => _configuration.description;
        public VConsumableRarity Rarity => _configuration.rarity;
        public VConsumableConfiguration _configuration;
        
        public VConsumableType type;
        
        public VConsumable(VConsumableConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}