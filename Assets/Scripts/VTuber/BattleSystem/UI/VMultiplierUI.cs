using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VMultiplierUI : VStatUI
    {
        
        [SerializeField] private TMP_Text MultiplierText;
        [SerializeField] private Transform grid;
        [SerializeField] private GameObject colorPrefab;
        
        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnMultiplierChange;
            SetFontStyle(MultiplierText, FontStyles.Bold);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnMultiplierSequenceCalculated, OnMultiplierSequenceCalculated);
        }



        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnMultiplierSequenceCalculated, OnMultiplierSequenceCalculated);
        }

        private void OnMultiplierSequenceCalculated(Dictionary<string, object> messagedict)
        {
            List<Color> colors = messagedict["Colors"] as List<Color>;
            foreach (var color in colors)
            {
                GameObject colorObj = Instantiate(colorPrefab, grid);
                colorObj.GetComponent<Image>().color = color;
            }
        }
        
        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            bool isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            bool shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            int delta = messagedict["Delta"] as int ? ?? 0;
            MultiplierText.text = $"提升率: {messagedict["NewValue"] as int? ?? 0}%";
            if(delta == 0)
                return;
            
            _animationQueue.Enqueue(AnimationType.Punch, transform, () =>
            {
                RaiseEvents(isFromCard, shouldPlayTwice);
                MultiplierText.faceColor = Color.white;
            });
            MultiplierText.faceColor = delta > 0 ? Color.green : Color.red;
        }
    }
}