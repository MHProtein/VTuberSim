using System.Collections.Generic;

namespace VTuber.Core.EventCenter
{
    public delegate void FunctionWithADict(Dictionary<string, object> messageDict);
    public class VRootEventCenter : VEventCenter<VRootEventCenter, string, FunctionWithADict>
    {
        
    }
}