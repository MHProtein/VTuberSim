using System.Linq;
using VTuber.BattleSystem.UI;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Core.RaisingEffect.RaisingAddRandomCardEffect
{
    public class VRaisingAddRandomCardEffect : VRaisingEffect
    {
        private VCardCondition _condition;
        public VRaisingAddRandomCardEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            var configs = VSingleton<VBattleDataManager>.Instance.GetAllCardConfigurations().
                Where(configuration => _condition.IsTrue(configuration.CreateCard())).ToList();
            
            if(configs.Count == 0)
                return;
            
            var randomIndex = UnityEngine.Random.Range(0, configs.Count);
            character.CardLibrary.AddCard(configs[randomIndex].CreateCard());
        }
    }
}