using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace VTuber.BattleSystem.UI
{


    public class VAnimationQueue
    {
        private readonly Queue<Sequence> queue = new Queue<Sequence>();

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

    }

}