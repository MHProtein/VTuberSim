using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.RaisingEffect;

namespace VTuber.Consumable
{
    public class VConsumableManager
    {
        List<VConsumable> consumables = new List<VConsumable>();
        private VCharacter _character;
        private VBattle _battle;
        
        public bool CanUseConsumable => _canUseConsumable;
        private bool _canUseConsumable;

        public void AddConsumable(VConsumable consumable, VCharacter character)
        {
            consumables.Add(consumable);
            _character = character;
        }
        
        public void RemoveConsumable(VConsumable consumable)
        {
            consumables.Remove(consumable);
        }
        
        public void SetBattle(VBattle battle)
        {
            _battle = battle;
        }
        
        public void ClearBattle()
        {
            _battle = null;
        }

        public bool CanUseBattleConsumable()
        {
            return _battle is not null;
        }

        public void SetCanUseConsumable(bool canUseConsumable)
        {
            _canUseConsumable = canUseConsumable;
        }

        public void ApplyBattleEffects(List<VEffect> effects)
        {
            effects.ForEach(effect => effect.ApplyEffect(_battle));
        }
        
        public void ApplyRaisingEffects(List<VRaisingEffect> effects)
        {
            effects.ForEach(effect => effect.ApplyEffect(_character));
        }
    }
}