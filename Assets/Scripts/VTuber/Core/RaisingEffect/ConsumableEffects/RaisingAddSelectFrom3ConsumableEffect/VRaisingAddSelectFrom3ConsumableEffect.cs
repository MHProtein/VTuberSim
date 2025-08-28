using System;
using System.Collections.Generic;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddSelectFrom3ConsumableEffect : VRaisingConsumableEffect
    {
        private VCharacter _character;
        public VRaisingAddSelectFrom3ConsumableEffect(VRaisingConsumableEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            _character = character;
            var consumables = GetRandomConsumables(3, character.LiveType);
            
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginSelectConsumableFrom3, new Dictionary<string, object>()
            {
                {"Consumables", consumables },
                {"Action", new Action<VConsumable>(AddConsumable)}
            });
        }

        public override void Upgrade()
        {
            
        }

        public override void DownGrade()
        {
            
        }
        
        public void AddConsumable(VConsumable consumable)
        {
            _character.ConsumableManager.AddConsumable(consumable);
        }
        public override string GetParameter()
        {
            return "";
        }
    }
}