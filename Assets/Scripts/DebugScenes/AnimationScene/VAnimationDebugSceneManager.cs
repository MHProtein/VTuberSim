using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;
using VTuber.Core.UI;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace DebugScenes.AnimationScene
{
    public class VAnimationDebugSceneManager : VMonoBehaviour
    {
        [SerializeField] private VRaisingAnimationSystem raisingAnimationSystem;
        [SerializeField] private Transform queue;
        [SerializeField] private GameObject animationRequestPrefab;
        
        List<TMP_Text> animationRequestTexts = new List<TMP_Text>();
        
        public void AddCoopAnim()
        {
            raisingAnimationSystem.DebugEnqueueAnimationRequest(new VAnimationRequest
            {
                instigatorType = VInstigatorType.Ignore,
                instigatorIcon = null,
                animationType = VAnimationType.CoopUpgrade,
            });
            var x = Instantiate(animationRequestPrefab, queue).GetComponent<TMP_Text>();
            x.text = "协助者升级";
            animationRequestTexts.Add(x);
        }
        
        public void AddAttributeAnim()
        {
            raisingAnimationSystem.DebugEnqueueAnimationRequest(new VAnimationRequest
            {
                instigatorType = VInstigatorType.Ignore,
                animationType = VAnimationType.AttributeAnimation,
                attributeIcon = VUIUtils.Instance.GetRandomAttributeIcon(),
                value = Random.Range(-100, 100),
            });        
            var x = Instantiate(animationRequestPrefab, queue).GetComponent<TMP_Text>();
            x.text = "属性动画";
            animationRequestTexts.Add(x);
        }

        public void AddEffectCardAnim()
        {
            raisingAnimationSystem.DebugEnqueueAnimationRequest(new VAnimationRequest
            {
                instigatorType = VInstigatorType.Ignore,
                instigatorIcon = VUIUtils.Instance.GetCoopIcon(),
                animationType = VAnimationType.EffectCards,
                attributeIcon = VUIUtils.Instance.GetRandomAttributeIcon(),
                description = "效果卡动画占位占位占位占位",
            });
            var x = Instantiate(animationRequestPrefab, queue).GetComponent<TMP_Text>();
            x.text = "效果卡动画";
            animationRequestTexts.Add(x);
        }
        
        public void ExecuteAnimations()
        {
            raisingAnimationSystem.ExecuteAnimations(() => { });
            foreach (var requestText in animationRequestTexts)
            {
                Destroy(requestText.gameObject);
            }
            animationRequestTexts.Clear();
        }
    }
}