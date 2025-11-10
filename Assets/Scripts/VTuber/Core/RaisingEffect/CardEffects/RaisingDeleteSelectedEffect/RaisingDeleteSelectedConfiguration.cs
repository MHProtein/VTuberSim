using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingDeleteSelectedConfiguration : VRaisingEffectConfiguration
    {
        public VRaisingDeleteSelectedConfiguration(CellRange row) : base(row)
        {
        }

        public VCardCondition Condition { get; }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingDeleteSelected(this);
        }
    }
}