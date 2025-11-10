using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingDeleteSelected : VRaisingEffect
    {
        private VCharacter _character;
        private VCardCondition _condition;

        public VRaisingDeleteSelected(VRaisingDeleteSelectedConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            _character = character;
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginSelectCard, new Dictionary<string, object>
            {
                { "ActionType", VCardActionType.Delete }
            });
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