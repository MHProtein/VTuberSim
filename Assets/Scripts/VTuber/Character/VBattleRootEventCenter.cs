using System.Collections.Generic;
using VTuber.BattleSystem.Core;

namespace VTuber.Core.EventCenter
{
    public enum VRaisingEventKey
    {
        Default,
        
    }
    
    public class VRaisingRootEventCenter : VEventCenter<VRaisingRootEventCenter, VRaisingEventKey, FunctionWithADict>
    {
        
    }
}