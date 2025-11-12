using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;

namespace VTuber.BattleSystem.Core.UI
{
    public class VScriptSelectionMenu : VUIBehaviour
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private GridLayoutGroup content;
        [SerializeField] private Transform originalContnetPosition;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button prevButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button returnButton;
        [SerializeField] private GameObject scriptUIPrefab;
        public Action<VScriptConfiguration> _confirmAction;
        private bool _firstTimeShow;
        private int _index;
        private Vector3 _originalPosition;
        private Action _returnAction;
        private List<VScriptConfiguration> _scripts;
        private List<VScriptSelectionUI> _scriptUIs;
        private float contentSpacing;

        protected override void Awake()
        {
            base.Awake();
            contentSpacing = content.cellSize.x;
            nextButton.onClick.AddListener(Next);
            prevButton.onClick.AddListener(Prev);
            confirmButton.onClick.AddListener(Confirm);
            returnButton.onClick.AddListener(Return);
        }

        protected override void Start()
        {
            base.Start();
            _originalPosition = content.transform.localPosition;
        }

        private void Confirm()
        {
            _confirmAction?.Invoke(_scripts[_index]);
        }

        public void Return()
        {
            _returnAction?.Invoke();
        }

        public void Initialize(List<VScriptConfiguration> scripts, Action<VScriptConfiguration> confirmAction,
            Action returnAction)
        {
            content.transform.localPosition = _originalPosition;
            _firstTimeShow = true;
            _scripts = scripts;
            _scripts.Sort((script1, script2) => script1.index.CompareTo(script2.index));
            _confirmAction = confirmAction;
            _returnAction = returnAction;
            _index = 0;
            nextButton.interactable = _index < _scripts.Count - 1;
            prevButton.interactable = _index > 0;
            _scriptUIs = new List<VScriptSelectionUI>();
            foreach (var script in scripts)
            {
                var scriptUI = Instantiate(scriptUIPrefab, content.transform);
                var scriptSelectionUI = scriptUI.GetComponent<VScriptSelectionUI>();
                scriptSelectionUI.ShowScript(script);
                _scriptUIs.Add(scriptSelectionUI);
            }
        }

        public void Show()
        {
            ui.SetActive(true);
            nextButton.interactable = _index < _scripts.Count - 1;
            prevButton.interactable = _index > 0;
            if (_firstTimeShow)
            {
                _firstTimeShow = false;
                confirmButton.interactable = false;
                Tween.LocalPosition(content.transform,
                    content.transform.localPosition - new Vector3(contentSpacing, 0, 0), 0.5f).OnComplete(
                    () => { confirmButton.interactable = true; });
            }
        }

        public void Hide()
        {
            ui.SetActive(false);
        }

        public void Next()
        {
            _index++;
            confirmButton.interactable = false;
            nextButton.interactable = false;
            prevButton.interactable = false;
            Tween.LocalPosition(content.transform, content.transform.localPosition - new Vector3(contentSpacing, 0, 0),
                0.5f).OnComplete(
                () =>
                {
                    nextButton.interactable = _index < _scripts.Count - 1;
                    prevButton.interactable = _index > 0;
                    confirmButton.interactable = true;
                });
        }

        public void Prev()
        {
            _index--;
            confirmButton.interactable = false;
            nextButton.interactable = false;
            prevButton.interactable = false;
            Tween.LocalPosition(content.transform, content.transform.localPosition + new Vector3(contentSpacing, 0, 0),
                0.5f).OnComplete(
                () =>
                {
                    confirmButton.interactable = true;
                    nextButton.interactable = _index < _scripts.Count - 1;
                    prevButton.interactable = _index > 0;
                });
        }

        public void Clear()
        {
            foreach (var ui in _scriptUIs) Destroy(ui.gameObject);
            _scriptUIs.Clear();
        }
    }
}