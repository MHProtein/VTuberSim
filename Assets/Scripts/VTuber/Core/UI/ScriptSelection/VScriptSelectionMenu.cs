using System;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.UI;
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
        private float contentSpacing;
        private List<VScriptConfiguration> _scripts;
        private int _index;
        public Action<VScriptConfiguration> _confirmAction;
        private Action _returnAction;
        private bool _firstTimeShow;

        protected override void Awake()
        {
            base.Awake();
            contentSpacing = content.cellSize.x;
            nextButton.onClick.AddListener(Next);
            prevButton.onClick.AddListener(Prev);
            confirmButton.onClick.AddListener(Confirm);
            returnButton.onClick.AddListener(Return);
        }

        private void Confirm()
        {
            _confirmAction?.Invoke(_scripts[_index]);
        }        
        
        public void Return()
        {
            _returnAction?.Invoke();
        }

        public void Initialize(List<VScriptConfiguration> scripts, Action<VScriptConfiguration> confirmAction, Action returnAction)
        {
            _firstTimeShow = true;
            _scripts = scripts;
            _confirmAction = confirmAction;
            _returnAction = returnAction;
            _index = 0;
            nextButton.interactable = _index < _scripts.Count - 1;
            prevButton.interactable = _index > 0;
            foreach (var script in scripts)
            {
                var scriptUI = Instantiate(scriptUIPrefab, content.transform);
                var scriptSelectionUI = scriptUI.GetComponent<VScriptSelectionUI>();
                scriptSelectionUI.ShowScript(script);
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
                Tween.LocalPosition(content.transform, content.transform.localPosition - new Vector3(contentSpacing, 0, 0), 0.5f).OnComplete(
                    () =>
                    {
                        confirmButton.interactable = true;
                    });
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
            nextButton.interactable = _index < _scripts.Count - 1;
            prevButton.interactable = _index > 0;
            Tween.LocalPosition(content.transform, content.transform.localPosition - new Vector3(contentSpacing, 0, 0), 0.5f).OnComplete(
                () =>
                {
                    confirmButton.interactable = true;
                });
        }

        public void Prev()
        {
            _index--;
            confirmButton.interactable = false;
            nextButton.interactable = _index < _scripts.Count - 1;
            prevButton.interactable = _index > 0;
            Tween.LocalPosition(content.transform, content.transform.localPosition + new Vector3(contentSpacing, 0, 0), 0.5f).OnComplete(
                () =>
                {
                    confirmButton.interactable = true;
                });
        }
    }
}