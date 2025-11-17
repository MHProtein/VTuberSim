using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.Foundation;
using VTuber.Core.SE;
using VTuber.EventSystem.UI;
using VTuber.RaisingAnimationSystem.Animations.SelectFrom3ConsumableAnimation;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Dialogue.UI
{
    public class VEventSystemUI : VSingletonMonobehaviour<VEventSystemUI>
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private VideoPlayer loadingVideoPlayer;
        [SerializeField] private Transform normalScreenPosition;
        [SerializeField] private RectTransform uiWrappers;
        [SerializeField] private RectTransform battleUIWrapper;
        [SerializeField] private RectTransform eventUIWrapper;
        [SerializeField] private VPhaseEndingSelectionMenu phaseEndingSelectionMenu;
        [SerializeField] private GameObject endingOptionPrefab;

        [SerializeField] private VSelectFrom3ConsumablesMenu selectFrom3ConsumablesMenu;

        private Action _closePhaseEndingSelectionMenuAction;
        private Action _closeCardLibrary;
        private Action _CloseSelectFrom3Menu;
        private Action _CloseSelectFrom3ConsumablesMenu;
        private Action onVideoFinish;
        private bool _isFullScreen;

        protected override void Awake()
        {
            base.Awake();

            loadingVideoPlayer.loopPointReached += OnLoadingEnded;
        }

        public void SetFullScreenButton()
        {
            SetFullScreen();
        }

        public Tween SetFullScreen()
        {
            _isFullScreen = !_isFullScreen;
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Raising_ZoomInOut);
            if (_isFullScreen)
            {
                canvas.sortingOrder = 2;
                Tween.UIAnchoredPosition(uiWrappers, Vector3.zero, 0.3f);
                return Tween.Scale(uiWrappers, Vector3.one, 0.3f);
            }

            Tween.Position(uiWrappers, normalScreenPosition.position, 0.3f).OnComplete(() =>
            {
                canvas.sortingOrder = -1;
            });
            return Tween.Scale(uiWrappers, Vector3.one * 0.6f, 0.3f);
        }

        private void OnLoadingEnded(VideoPlayer source)
        {
            loadingVideoPlayer.gameObject.SetActive(false);
            loadingVideoPlayer.Stop();
            onVideoFinish?.Invoke();
            onVideoFinish = null;
        }

        public void PlayVideo(Action onFinish)
        {
            loadingVideoPlayer.gameObject.SetActive(true);
            loadingVideoPlayer.Play();
            onVideoFinish = onFinish;
        }

        public void InitializePhaseEndingSelectionMenu(List<VStreamEvent> endings, Action confirmAction)
        {
            phaseEndingSelectionMenu.gameObject.SetActive(true);
            phaseEndingSelectionMenu.Initialize(endingOptionPrefab, endings);
            _closePhaseEndingSelectionMenuAction = confirmAction;
        }

        public void ClosePhaseEndingSelectionMenu()
        {
            phaseEndingSelectionMenu.gameObject.SetActive(false);
            _closePhaseEndingSelectionMenuAction?.Invoke();
        }

        public void CloseSelectFrom3ConsumablesMenu()
        {
            selectFrom3ConsumablesMenu.gameObject.SetActive(false);
            _CloseSelectFrom3ConsumablesMenu?.Invoke();
        }

        public void OpenEventUI()
        {
            eventUIWrapper.gameObject.SetActive(true);
            battleUIWrapper.gameObject.SetActive(false);
        }

        public void OpenBattleUI()
        {
            eventUIWrapper.gameObject.SetActive(false);
            battleUIWrapper.gameObject.SetActive(true);
        }

        public void CloseUI()
        {
            eventUIWrapper.gameObject.SetActive(false);
            battleUIWrapper.gameObject.SetActive(false);
        }
    }
}