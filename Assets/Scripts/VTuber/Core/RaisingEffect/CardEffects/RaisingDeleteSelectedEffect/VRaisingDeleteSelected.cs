using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingDeleteSelected : VRaisingEffect
    {
        private VCardCondition _condition;
        private VCharacter _character;
        public VRaisingDeleteSelected(VRaisingDeleteSelectedConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            _character = character;
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginSelectCard, new Dictionary<string, object>()
            {
                {"Action", new Action<VCard>(DeleteCard)}
            });
        }

        public override void Upgrade()
        {
            
        }

        public override void DownGrade()
        {
        }

        public void DeleteCard(VCard selectedCard)
        {
            _character.CardLibrary.RemoveCard(selectedCard);
            
        }
        public override string GetParameter()
        {
            return "";
        }
    }
}