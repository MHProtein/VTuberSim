using System;
using Spire.Xls;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect.RaisingDeleteSelectedEffect;

namespace VTuber.Core.RaisingEffect
{
    public class RaisingDeleteSelectedConfiguration : VRaisingEffectConfiguration
    {
        public VCardCondition Condition => _condition;
        private VCardCondition _condition;
        
        public RaisingDeleteSelectedConfiguration(CellRange row) : base(row)
        {
            
            _condition =
                VResourcesManager.Instance.GetCardConditionByID(
                    Convert.ToUInt32(row.Columns[VRaisingEffectHeaderIndex.Condition].Value));
        }

        public override VRaisingEffect CreateEffect(string parameter)
        {
            return new VRaisingDeleteSelected(this);
        }
    }
}