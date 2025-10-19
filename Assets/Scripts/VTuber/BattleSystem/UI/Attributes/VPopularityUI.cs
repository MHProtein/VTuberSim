using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.SE;

namespace VTuber.BattleSystem.UI
{
    public class VBattlePopularityUI : VBattleAttributeUI
    {
        [SerializeField] private TMP_Text popularityText;
        [SerializeField] private TMP_Text targetText;
        [SerializeField] private Image bar;
        private int _target;
        private int _extraTarget;
        private bool _isPhaseEnding;

        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnPopularityChange;
            SetFontStyle(popularityText, FontStyles.Bold);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleUIInitialize, OnBattleUIInitialize);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleUIInitialize, OnBattleUIInitialize);
        }
        
        private void OnBattleUIInitialize(Dictionary<string, object> messagedict)
        {
            _isPhaseEnding = messagedict["IsPhaseEnding"] as bool? ?? false;
            _target = (int)messagedict["TargetPopularity"];
            _extraTarget = (int)messagedict["ExtraTargetPopularity"];
            popularityText.text = "0";
            targetText.text = _target.ToString();
            bar.fillAmount = 0;
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            bool isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            bool shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            int delta = messagedict["Delta"] as int ? ?? 0;
            var value = messagedict["NewValue"] as int? ?? 0;
            popularityText.text = $"{value}";
            
            popularityText.color = delta > 0 ? Color.green : Color.red;
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Battle_PopularityIncrease);
            _animationQueue.Enqueue(Tween.PunchScale(popularityText.transform, Vector3.one * 1.3f, 0.4f).OnComplete((
                () =>
                {
                    RaiseEvents(isFromCard, shouldPlayTwice);
                    popularityText.color = Color.white;
                })));

            if (!_isPhaseEnding)
            {
                if (value <= _target)
                {
                    Tween.UIFillAmount(bar, Mathf.Clamp((float)value / _target, 0.0f, 1.0f), 0.3f);
                    targetText.text = _target.ToString();
                }
                else
                {
                    Tween.UIFillAmount(bar, Mathf.Clamp((float)(value - _target) / (_extraTarget - _target), 0.0f, 1.0f), 0.3f);
                    targetText.text = _extraTarget.ToString();
                }
            }
            else
            {
                Tween.UIFillAmount(bar, Mathf.Clamp((float)value / _target, 0.0f, 1.0f), 0.3f);
                targetText.text = _target.ToString();
            }
        }
    }
}