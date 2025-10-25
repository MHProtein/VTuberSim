using System.Collections.Generic;
using PrimeTween;

namespace VTuber.BattleSystem.UI
{
    public class VAnimationQueue
    {
        private readonly Queue<Sequence> queue = new();

        public void Enqueue(Tween tween)
        {
            var seq = Sequence.Create()
                .Chain(tween)
                .ChainCallback(OnSequenceComplete);

            queue.Enqueue(seq);
        }

        private void OnSequenceComplete()
        {
            queue.Dequeue();
        }

        public void Clear()
        {
            while (queue.Count > 0) queue.Dequeue().Stop();
        }
    }
}