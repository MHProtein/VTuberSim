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
        [SerializeField] private Image arrow;

        private float arrowHeight;
        private float arrowWidth;
        
        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnMultiplierChange;
            SetFontStyle(MultiplierText, FontStyles.Bold);
            // arrowHeight = arrow.rectTransform.rect.height;
            // arrowWidth = arrow.rectTransform.rect.width;
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
            if (colors is null)
                return;
            for (int i = 0; i < colors.Count; i++)
            {       
                GameObject colorObj = Instantiate(colorPrefab, grid);
                colorObj.GetComponent<Image>().color = colors[i];
                // if (i == 0)
                // {
                //     arrow.transform.SetParent(colorObj.transform);
                //     arrow.transform.position = new Vector3(0, -arrowHeight, 0);
                // }
            }
        }
        
        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            MultiplierText.text = $"提升率: {messagedict["NewValue"] as int? ?? 0}%";
            
            _animationQueue.Enqueue(AnimationType.Punch, MultiplierText.transform);
            MultiplierText.faceColor = (Color)messagedict["Color"];
            
        }
    }
}