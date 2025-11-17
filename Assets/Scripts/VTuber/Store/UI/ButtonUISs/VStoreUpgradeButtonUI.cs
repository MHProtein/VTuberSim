using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.Core.EventCenter;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Store.UI
{
    public class VStoreUpgradeButtonUI : VStoreButtonUI
    {
        public override void OnClick()
        {
            base.OnClick();
            VRaisingAnimationSystem.Instance.
                EnqueueAnimationRequest(VAnimationRequestFactory.CreateSelectCardRequest(character.CardLibrary.GetCards().Where(card => !card.IsUpgraded).ToList(),
                    true, true, VAnimationType.UpgradeCard,
                    card =>
                    {
                        VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndUpgradeCard,
                            new Dictionary<string, object>
                            {
                                { "Upgraded", true },
                                { "UpgradedCard", card }
                            });
                        Buy();
                    },
                    () =>
                    {
                        VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndUpgradeCard,
                            new Dictionary<string, object>
                            {
                                { "Upgraded", false }
                            });
                    },
                    card => card.Upgrade(false, false))
                );
            
            VRaisingAnimationSystem.Instance.ExecuteAnimations(null);
        }
    }
}