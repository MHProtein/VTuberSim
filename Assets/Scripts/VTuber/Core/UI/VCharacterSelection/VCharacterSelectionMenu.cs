using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core.UI.VCharacterSelection
{
    public class VCharacterSelectionMenu : VUIBehaviour
    {
        [SerializeField] private GameObject ui;
        [FormerlySerializedAs("characterDetailsUI")] [SerializeField] private VCharacterDetailsUI details;
        [SerializeField] private Transform detailsPosition;
        [SerializeField] private Transform detailsHiddenPosition;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button prevButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button returnButton;
        
        private List<VCharacterConfiguration> characters;
        private int _index;
        private Action<VCharacterConfiguration> _confirmAction;
        private Action _returnAction;
        private bool firstTimeShow;
        protected override void Awake()
        {
            base.Awake();
            nextButton.onClick.AddListener(Next);
            prevButton.onClick.AddListener(Prev);
            confirmButton.onClick.AddListener(Confirm);
            returnButton.onClick.AddListener(Return);
        }

        public void Initialize(List<VCharacterConfiguration> characters, Action<VCharacterConfiguration> confirmAction, Action returnAction)
        {
            firstTimeShow = true;
            this.characters = characters;
            _index = 0;
            _confirmAction = confirmAction;
            _returnAction = returnAction;
        }

        public void Show()
        {
            if (firstTimeShow)
                firstTimeShow = false;
            details.transform.localPosition = detailsHiddenPosition.localPosition;
            details.SetDetails(characters[_index]);
            ui.SetActive(true);
            nextButton.interactable = _index < characters.Count - 1;
            prevButton.interactable = _index > 0;
            confirmButton.interactable = false;
            Tween.LocalPosition(details.transform, detailsPosition.localPosition, 0.5f).OnComplete(
                () =>
                {
                    confirmButton.interactable = true;
                });
            
        }

        public void Hide()
        {
            ui.SetActive(false);
        }
        
        public void Confirm()
        {
            _confirmAction?.Invoke(characters[_index]);
        }

        public void Return()
        {
            _returnAction?.Invoke();
        }
        
        public void Next()
        {
            _index++;
            confirmButton.interactable = false;
            nextButton.interactable = _index < characters.Count - 1;
            prevButton.interactable = _index > 0;
            
            Tween.LocalPosition(details.transform, detailsHiddenPosition.localPosition, .5f).OnComplete(
                () =>
                {
                    details.SetDetails(characters[_index]);
                    
                }).Chain(Tween.LocalPosition(details.transform, detailsPosition.localPosition, 0.5f).OnComplete(
                () =>
                {
                    confirmButton.interactable = true;
                }));
        }

        public void Prev()
        {
            _index--;
            confirmButton.interactable = false;
            nextButton.interactable = _index < characters.Count - 1;
            prevButton.interactable = _index > 0;
            
            Tween.LocalPosition(details.transform, detailsHiddenPosition.localPosition, .75f).OnComplete(
                () =>
                {
                    details.SetDetails(characters[_index]);
                    
                }).Chain(Tween.LocalPosition(details.transform, detailsPosition.localPosition, 0.75f).OnComplete(
                () =>
                {
                    confirmButton.interactable = true;
                }));
        }
    }
}