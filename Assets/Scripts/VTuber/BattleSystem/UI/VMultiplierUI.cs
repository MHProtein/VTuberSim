using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class vVMultiplierUI : VStatUI
    {
        
        [SerializeField] private TMP_Text MultiplierText;
        [SerializeField] private Transform grid;
        [SerializeField] private GameObject colorPrefab;
        [SerializeField] private Image arrow;

        private float arrowHeight;
        private float arrowWidth;

        private float blockHeight;
        private float blockWidth;

        private Sequence textSequence;
        private Sequence arrowSequence;
        
        private List<Image> colorObjects = new List<Image>();
        int arrowIndex = 0;
        private float initSize = 0;
        
        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnMultiplierChange;
            SetFontStyle(MultiplierText, FontStyles.Bold);
            arrowHeight = arrow.rectTransform.rect.height;
            arrowWidth = arrow.rectTransform.rect.width;
            textSequence = Sequence.Create();
            arrowSequence = Sequence.Create();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnMultiplierSequenceCalculated, OnMultiplierSequenceCalculated);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleBegin, OnBattleBegin);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnChange, OnTurnChange);
            
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnMultiplierSequenceCalculated, OnMultiplierSequenceCalculated);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleBegin, OnBattleBegin);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnChange, OnTurnChange);
        }
        
        private void OnTurnChange(Dictionary<string, object> messagedict)
        {
            int delta = (int)messagedict["Delta"];
            if (delta <= 0)
                return;

            for (int i = 0; i < delta; i++)
            {
                GameObject colorObj = Instantiate(colorPrefab, grid);
                var image = colorObj.GetComponent<Image>();
                image.color = colorObjects.Last().color;
                colorObjects.Add(image);
                float scale = initSize / (colorObjects.Count * blockWidth);
                if (!arrowSequence.isAlive)
                {
                    arrowSequence = Sequence.Create();
                }
                
                arrowSequence.Chain(Tween.Scale(grid.transform, new Vector3(scale, 1, 1), 0.2f));


                StartCoroutine(DelayMoveArrow());
            }
        }

        public IEnumerator DelayMoveArrow()
        {
            yield return new WaitForSeconds(0.2f);
            arrowSequence.Chain(Tween.Position(arrow.transform, colorObjects[arrowIndex - 1].transform.position + 
                                                                new Vector3(0, -arrowHeight, 0), 0.2f));
        }
        
        private void OnBattleBegin(Dictionary<string, object> messagedict)
        {
            Tween.Delay(0.1f, () =>
            {
                arrowIndex++;
                arrow.transform.position = colorObjects[0].transform.position + new Vector3(0, -arrowHeight, 0);
                initSize = colorObjects[0].rectTransform.rect.width * colorObjects.Count;
                blockHeight = colorObjects[0].rectTransform.rect.height;
                blockWidth = colorObjects[0].rectTransform.rect.width;
            });
        }
        
        private void OnMultiplierSequenceCalculated(Dictionary<string, object> messagedict)
        {
            List<Color> colors = messagedict["Colors"] as List<Color>;
            if (colors is null)
                return;
            for (int i = 0; i < colors.Count; i++)
            {       
                GameObject colorObj = Instantiate(colorPrefab, grid);
                var image = colorObj.GetComponent<Image>();
                image.color = colors[i];
                colorObjects.Add(image);
            }
        }
        
        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            MultiplierText.text = $"提升率: {messagedict["NewValue"] as int? ?? 0}%";
            MultiplierText.faceColor = (Color)messagedict["Color"];
            
            if (!textSequence.isAlive)
            {
                textSequence = Sequence.Create();
            }
            textSequence.Chain(Tween.PunchScale(MultiplierText.transform, Vector3.one * 1.3f, 0.5f));
        }
        
        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            if (!arrowSequence.isAlive)
            {
                arrowSequence = Sequence.Create();
            }
            arrowSequence.Chain(Tween.Position(arrow.transform, colorObjects[arrowIndex].transform.position + 
                                                                new Vector3(0, -arrowHeight, 0), 0.2f));
            arrowIndex++;
        }
    }
}