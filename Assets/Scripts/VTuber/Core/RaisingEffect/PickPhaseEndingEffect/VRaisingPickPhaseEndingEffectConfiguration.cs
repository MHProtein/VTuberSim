using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingPickPhaseEndingEffectConfiguration : VRaisingEffectConfiguration
    {
        public VRaisingPickPhaseEndingEffectConfiguration(CellRange row) : base(row)
        {
            
        }

        public override VRaisingEffect CreateEffect(string parameter)
        {
            return new VRaisingPickPhaseEndingEffect(this);
        }
    }
}