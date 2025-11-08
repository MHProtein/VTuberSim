using System.Collections.Generic;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;

namespace VTuber.ScheduleSystem.UI.RaisingAnimationSystem
{
    public enum VAnimationType
    {
        
    }
    
    public class VAnimationRequest
    {
        public VRaisingEffect effect;
        public VAnimationType animationType;
    }
    
    public class VRaisingAnimationSystem : VMonoBehaviour
    {
        private Dictionary<VAnimationType, VRaisingAnimation> _animations;
        private Queue<VAnimationRequest> _animationRequestQueue;

        public void Initialize()
        {
            _animationRequestQueue = new();
        }

        public void AddAnimationRequest(VAnimationRequest request)
        {
            _animationRequestQueue.Enqueue(request);
        }
        
        
    }
}