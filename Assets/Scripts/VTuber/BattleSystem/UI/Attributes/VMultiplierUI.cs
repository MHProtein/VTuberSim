using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.UI
{
    public class VBattleMultiplierUI : VBattleAttributeUI
    {
        [SerializeField] private TMP_Text MultiplierText;
        [SerializeField] private Transform grid;
        [SerializeField] private GameObject colorPrefab;
        [SerializeField] private Image arrow;

        private readonly List<Image> colorObjects = new();
        private string _attributeName = "";

        private float arrowHeight;
        private int arrowIndex = -1;
        private VAnimationQueue arrowSequence;
        private float arrowWidth;

        private float blockHeight;
        private float blockWidth;
        private float initSize;

        private VAnimationQueue textSequence;

        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnMultiplierChange;
            SetFontStyle(MultiplierText, FontStyles.Bold);
            arrowHeight = arrow.rectTransform.rect.height;
            arrowWidth = arrow.rectTransform.rect.width;

            textSequence = new VAnimationQueue();
            arrowSequence = new VAnimationQueue();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnMultiplierSequenceCalculated,
                OnMultiplierSequenceCalculated);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnMultiplierSequenceCalculated,
                OnMultiplierSequenceCalculated);
        }

        private void OnRotateMultiplier(Dictionary<string, object> messagedict)
        {
            _attributeName = (string)messagedict["Name"];
            MultiplierText.text = $"提升率: {messagedict["NewValue"] as int? ?? 0}%";

            textSequence.Enqueue(Tween.PunchScale(MultiplierText.transform, Vector3.one * 1.3f, 0.5f));
            if (!messagedict.ContainsKey("Color"))
                return;
            MultiplierText.color = (Color)messagedict["Color"];
        }

        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnRotateMultiplier, OnRotateMultiplier);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleBegin, OnBattleBegin);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnChange, OnTurnChange);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
            foreach (var colorObject in colorObjects) Destroy(colorObject.gameObject);
            colorObjects.Clear();
            arrowIndex = -1;
        }

        private void OnTurnChange(Dictionary<string, object> messagedict)
        {
            var delta = (int)messagedict["Delta"];
            if (delta <= 0)
                return;

            for (var i = 0; i < delta; i++)
            {
                var colorObj = Instantiate(colorPrefab, grid);
                var image = colorObj.GetComponent<Image>();
                image.color = colorObjects.Last().color;
                colorObjects.Add(image);
                var scale = initSize / (colorObjects.Count * blockWidth);

                arrowSequence.Enqueue(Tween.Scale(grid.transform, new Vector3(scale, 1, 1), 0.2f));

                StartCoroutine(DelayMoveArrow());
            }
        }

        public IEnumerator DelayMoveArrow()
        {
            yield return new WaitForSeconds(0.2f);
            if (arrowIndex >= colorObjects.Count || arrowIndex < 0)
                yield break;
            arrowSequence.Enqueue(Tween.LocalPosition(arrow.transform,
                colorObjects[arrowIndex].transform.localPosition +
                new Vector3(0, -arrowHeight, 0), 0.2f));
        }

        private void OnBattleBegin(Dictionary<string, object> messagedict)
        {
            Tween.Delay(0.1f, () =>
            {
                arrow.transform.position = colorObjects[0].transform.position + new Vector3(0, -arrowHeight, 0);
                initSize = colorObjects[0].rectTransform.rect.width * colorObjects.Count;
                blockHeight = colorObjects[0].rectTransform.rect.height;
                blockWidth = colorObjects[0].rectTransform.rect.width;
            });
        }

        private void OnMultiplierSequenceCalculated(Dictionary<string, object> messagedict)
        {
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleBegin, OnBattleBegin);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnChange, OnTurnChange);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnRotateMultiplier, OnRotateMultiplier);

            var colors = messagedict["Colors"] as List<Color>;
            if (colors is null)
                return;
            for (var i = 0; i < colors.Count; i++)
            {
                var colorObj = Instantiate(colorPrefab, grid);
                var image = colorObj.GetComponent<Image>();
                image.color = colors[i];
                colorObjects.Add(image);
            }

            if (messagedict.TryGetValue("Index", out var indexObj))
            {
                arrowIndex = (int)indexObj;
            }
            else
            {
                arrowIndex++;
                if (arrowIndex >= colorObjects.Count)
                    return;
            }

            StartCoroutine(DelayMoveArrow());
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            if (!_attributeName.Equals((string)messagedict["Name"]))
                return;
            MultiplierText.text = $"提升率: {messagedict["NewValue"] as int? ?? 0}%";

            textSequence.Enqueue(Tween.PunchScale(MultiplierText.transform, Vector3.one * 1.3f, 0.5f));
        }

        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            arrowIndex++;
            if (arrowIndex >= colorObjects.Count)
                return;

            StartCoroutine(DelayMoveArrow());
        }
    }
}