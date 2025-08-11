using System.Collections.Generic;
using VTuber.Core.EventCenter;

namespace VTuber.CoopSystem
{
    public class VCooperatorManager
    {
        List<VCooperator> coopOperators = new List<VCooperator>();

        public void AddCooperator(VCooperatorConfiguration configuration)
        {
            var cooperator = new VCooperator(configuration);
            coopOperators.Add(cooperator);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCooperatorAdded, new Dictionary<string, object> { { "Cooperator", cooperator } });
        }
        
    }
}