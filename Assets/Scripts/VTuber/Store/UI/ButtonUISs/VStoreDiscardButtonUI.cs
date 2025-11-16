using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.Core.EventCenter;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Store.UI
{
    public class VStoreDiscardButtonUI : VStoreButtonUI
    {
        public override void OnClick()
        {
            VRaisingAnimationSystem.Instance.
                EnqueueAnimationRequest(VAnimationRequestFactory.CreateSelectCardRequest(character.CardLibrary.GetCards(),
                    true, true, VAnimationType.RemoveCard,
                    card =>
                        {
                            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndDeleteCard,
                                new Dictionary<string, object>
                                {
                                    { "Deleted", true },
                                    { "DeletedCard", card }
                                });

                            Buy();
                        },
                    () =>
                    {
                        VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndDeleteCard,
                            new Dictionary<string, object>
                            {
                                { "Deleted", false }
                            });
                    }));
            VRaisingAnimationSystem.Instance.ExecuteAnimations(null);
        }
    }
}