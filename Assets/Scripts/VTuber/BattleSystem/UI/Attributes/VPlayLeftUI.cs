using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.UI
{
    public class VBattlePlayLeftUI : VBattleAttributeUI
    {
        [SerializeField] private TMP_Text PlayLeftText;
        [SerializeField] private GameObject textObject;

        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnPlayLeftChange;
            SetFontStyle(PlayLeftText, FontStyles.Bold);
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            var isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            var shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            var delta = messagedict["Delta"] as int? ?? 0;
            var value = messagedict["NewValue"] as int? ?? 0;

            if (value > 1)
            {
                textObject.gameObject.SetActive(true);
            }

            PlayLeftText.text = $"{value}";

            if (delta == 0)
                return;

            PlayLeftText.color = delta > 0 ? Color.green : Color.red;
            _animationQueue.Enqueue(Tween.PunchScale(transform, Vector3.one * 1.3f, 0.4f).OnComplete(() =>
            {
                RaiseEvents(isFromCard, shouldPlayTwice);
                PlayLeftText.faceColor = Color.white;
                if (value <= 1)
                {
                    textObject.gameObject.SetActive(false);
                }
            }));

        }
    }
}