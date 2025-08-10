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

        public override void ApplyEffect(VCharacter character)
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginSelectCard, new Dictionary<string, object>()
            {
                {"Action", new Action<VCard>(ReplaceCard)}
            });
        }
        
        public void ReplaceCard(VCard selectedCard)
        {
            _character.CardLibrary.RemoveCard(selectedCard);
            
        }
    }
}