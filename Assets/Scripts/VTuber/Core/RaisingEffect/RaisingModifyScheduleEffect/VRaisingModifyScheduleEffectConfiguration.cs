using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingModifyScheduleEffectConfiguration : VRaisingEffectConfiguration
    {
        public VRaisingModifyScheduleEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingModifyScheduleEffect(this);
        }
    }
}