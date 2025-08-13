using System.Linq;
using VTuber.BattleSystem.UI;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRandomCardEffect : VRaisingEffect
    {
        private VCardCondition _condition;
        public VRaisingAddRandomCardEffect(VRaisingAddRandomCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        public override void ApplyEffect(VCharacter character)
        {
            var configs = VSingleton<VResourcesManager>.Instance.GetAllCardConfigurations().
                Where(configuration => _condition.IsTrue(configuration)).ToList();
            
            if(configs.Count == 0)
                return;
            
            var randomIndex = UnityEngine.Random.Range(0, configs.Count);
            character.CardLibrary.AddCard(configs[randomIndex].CreateCard());
            VDebug.Log("Added random card: " + configs[randomIndex].cardName);
        }

        public override void Upgrade()
        {
            
        }

        public override void DownGrade()
        {
        }
    }
}