using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VPopularityUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text popularityText;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnPopularityChange,
                dict => { popularityText.text = $"Popularity: {dict["NewValue"] as int? ?? 0}"; });
        }

    }
}