using System;
using Spire.Xls;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddSelectedFrom3EffectConfiguration : VRaisingEffectConfiguration
    {
        public VCardCondition Condition => _condition;
        private VCardCondition _condition;
        
        public VRaisingAddSelectedFrom3EffectConfiguration(CellRange row) : base(row)
        {
            
            _condition =
                VResourcesManager.Instance.GetCardConditionByID(
                    Convert.ToUInt32(row.Columns[VRaisingEffectHeaderIndex.Condition].Value));
        }


        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingAddSelectedFrom3Effect(this);
        }
    }
}