using System.Collections.Generic;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Store;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingStoreGlobalDiscountEffect : VRaisingEffect
    {
        private readonly VUpgradableValue<float> _discount;

        public VRaisingStoreGlobalDiscountEffect(VRaisingEffectConfiguration configuration, string parameter,
            string upgradedParameter) : base(configuration)
        {
            _discount = new VUpgradableValue<float>(float.Parse(parameter.Trim()),
                float.Parse(upgradedParameter.Trim()));
            shouldPlayAnimation = false;
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            var store = messagedict["Store"] as VStore;
            store.SetGlobalDiscount(_discount.Value);
        }

        public override void Upgrade()
        {
            _discount.Upgrade();
        }

        public override void DownGrade()
        {
            _discount.Downgrade();
        }

        public override string GetParameter()
        {
            return (_discount.Value * 100f).ToString("0.0");
        }
    }
}