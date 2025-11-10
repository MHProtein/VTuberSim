using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRandomConsumableEffect : VRaisingConsumableEffect
    {
        private VCharacter _character;

        public VRaisingAddRandomConsumableEffect(VRaisingConsumableEffectConfiguration configuration) : base(
            configuration)
        {
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            _character = character;
            var consumable = GetRandomConsumables(1, character.LiveType).FirstOrDefault();
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnShowAddConsumable,
                new Dictionary<string, object>
                {
                    { "Consumable", consumable },
                    { "Action", new Action<VConsumable>(AddConsumable) }
                });
        }

        public void AddConsumable(VConsumable consumable)
        {
            _character.ConsumableManager.AddConsumable(consumable);
        }

        public override void Upgrade()
        {
        }

        public override void DownGrade()
        {
        }

        public override string GetParameter()
        {
            return "";
        }
    }
}