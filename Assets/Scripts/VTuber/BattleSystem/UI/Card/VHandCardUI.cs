using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VHandCardUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public VCardUI cardUI;
        public VBattleUI battleUI;

        public int index;
        public float inspectionY = 150.0f;

        public bool selected;
        public bool selfSelected;
        private float _deltaTime;

        private bool _inspectable;

        [Header("Inspection")] private Vector3 _inspectionScale;

        private bool _isMoving;
        private bool _isPlayable = true;
        private bool _isPointerStaying;
        private bool _isRotating;
        private bool _isScaling;

        private bool _moveWithMouse;
        private Vector3 _originalPosition;

        private int _originalSiblingIndex;

        private VAnimationQueue _popularityPreviewAnimationQueue;
        private float _positionSmoothTime;
        private Vector3 _positionVelocity;
        private float _rotationSmoothTime;

        private float _rotationVelocity;
        private float _scaleSmoothTime;
        private Vector3 _scaleVelocity;
        private VAnimationQueue _shieldPreviewAnimationQueue;

        private Vector3 _targetPosition;
        private Vector3 _targetRotation;

        private Vector3 _targetScale;
        public VCard card;

        private bool doubleCheck;

        private Dictionary<string, object> message;

        private bool selectClickUp;
        private bool shouldWaitSetPlayable;

        public VHandCardUI(Vector3 targetRotation, bool isScaling)
        {
            _targetRotation = targetRotation;
            _isScaling = isScaling;
        }

        public Vector3 OriginalScale { get; private set; }

        public Vector3 OriginalPosition => _originalPosition;

        public Vector3 OriginalRotation { get; private set; }


        protected override void Awake()
        {
            base.Awake();
            SetInteractive(false);
            _inspectionScale = new Vector3(1.0f, 1.0f, 1.0f);
            _originalSiblingIndex = transform.GetSiblingIndex();
            message = new Dictionary<string, object>();
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            _popularityPreviewAnimationQueue = new VAnimationQueue();
            _shieldPreviewAnimationQueue = new VAnimationQueue();
        }

        private void Update()
        {
            DetectDeselect();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_inspectable || !_isPlayable)
                return;
            if (eventData.button == PointerEventData.InputButton.Left && !selected)
                Select();
            else if (selfSelected && eventData.button == PointerEventData.InputButton.Left)
                Play();

            if (selected && !selfSelected)
            {
                battleUI.UnselectCurrent();
                Select();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerStaying = true;

            if (selected || !_inspectable)
                return;
            Inspect();
            //battleUI.MoveAway(index);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (selected || !_inspectable)
                return;

            _isPointerStaying = false;
            ExitInspection();
            //battleUI.MoveBack(index);
        }

        public void ToHandSlot(Vector3 position, Vector3 rotation, Vector3 scale, float smoothTime)
        {
            SetInteractive(false);
            SetPosition(position, smoothTime, true, () => { SetInteractive(!battleUI.IsCardApplying); });
            SetRotation(rotation, smoothTime, true);
            SetScale(scale, smoothTime, true);
        }

        public void MoveToDiscardPile(Vector3 targetPosition, float smoothTime)
        {
            SetInteractive(false);
            Tween.Position(transform, targetPosition, smoothTime, Ease.Linear);
            Tween.Scale(transform, Vector3.zero, smoothTime, Ease.Linear).OnComplete(DestroyGameObject);
        }

        private void DestroyGameObject()
        {
            _popularityPreviewAnimationQueue.Clear();
            _shieldPreviewAnimationQueue.Clear();
            if (gameObject)
                Destroy(gameObject);
        }

        public void SetCardPlayable(bool isPlayable)
        {
            if (!isPlayable)
            {
                _isPlayable = false;
                if (cardUI.background != null)
                    cardUI.background.color = Color.gray;
                return;
            }

            if (battleUI.IsCardApplying)
            {
                shouldWaitSetPlayable = true;
                return;
            }

            _isPlayable = true;
            if (cardUI.background != null)
                cardUI.background.color = Color.white;
        }

        public void SetInteractive(bool interactive)
        {
            _inspectable = interactive;
        }

        public void SetOriginalPosition(float smoothTime)
        {
            _isMoving = true;
            _targetPosition = _originalPosition;
            _positionSmoothTime = smoothTime;
        }

        public void SetPosition(Vector3 targetPosition, float smoothTime, bool setOriginal, Action action = null)
        {
            _isMoving = true;
            _targetPosition = targetPosition;
            _positionSmoothTime = smoothTime;
            if (setOriginal)
                _originalPosition = targetPosition;
            var tween = Tween.LocalPosition(transform, _targetPosition, _positionSmoothTime, Ease.Linear).OnComplete(
                () =>
                {
                    _isMoving = false;
                    if (action is not null)
                        action();
                });
        }


        public void SetScale(Vector3 targetScale, float smoothTime, bool setOriginal)
        {
            _isScaling = true;
            _targetScale = targetScale;
            _scaleSmoothTime = smoothTime;
            if (setOriginal)
                OriginalScale = _targetScale;
            Tween.Scale(transform, targetScale, smoothTime, Ease.Linear).OnComplete(
                () => { _isScaling = false; });
            ;
        }

        public void SetRotation(Vector3 targetAngle, float smoothTime, bool setOriginal)
        {
            _isRotating = true;
            _targetRotation = targetAngle;
            _rotationSmoothTime = smoothTime;
            if (setOriginal)
                OriginalRotation = _targetRotation;
            Tween.LocalRotation(transform, targetAngle, smoothTime, Ease.Linear).OnComplete(
                () => { _isRotating = false; });
            ;
        }

        private void Play()
        {
            Deselect();
            SetInteractive(false);
            card.Play();
            battleUI.PlayCardPlayedSFX();
        }

        private void DetectDeselect()
        {
            if (!selfSelected)
                return;

            if (!Input.GetMouseButtonDown(1))
                return;

            Deselect();
        }

        private void Inspect()
        {
            if (!_inspectable || !_isPlayable)
                return;

            var pos = new Vector3(_originalPosition.x, inspectionY, _originalPosition.z);
            // SetPosition(pos, _positionSmoothTime, false);
            //
            // SetRotation(Vector3.zero, _rotationSmoothTime, false);
            //
            // SetScale(_inspectionScale, _scaleSmoothTime, false);

            transform.SetAsLastSibling();
        }

        private void ExitInspection()
        {
            // SetPosition(_originalPosition, _positionSmoothTime, false);
            //
            // SetRotation(_originalRotation, _rotationSmoothTime, false);
            //
            // SetScale(_originalScale, _scaleSmoothTime, false);
            transform.SetSiblingIndex(_originalSiblingIndex);
        }

        private void Select()
        {
            Inspect();
            selected = true;
            selfSelected = true;
            cardUI.background.color = Color.cyan;
            battleUI.Selected(true);
            SetPosition(transform.localPosition + Vector3.up * 50, 0.1f, false);
            battleUI.PlayCardSelectedSFX();
        }

        public void Deselect()
        {
            selfSelected = false;
            selectClickUp = false;
            cardUI.background.color = Color.white;
            battleUI.Selected(false);
            SetPosition(_originalPosition, 0.08f, false);

            ExitInspection();
        }

        public void OnCardStopApplying()
        {
            if (shouldWaitSetPlayable) SetCardPlayable(true);
        }

        public void SetPopularityPreview(bool isFirstTime, int originalValue, int finalValue)
        {
            if (originalValue == 0)
            {
                if (cardUI.popularityText != null)
                    cardUI.popularityText.gameObject.SetActive(false);
                if (cardUI.popularityImage != null)
                    cardUI.popularityImage.gameObject.SetActive(false);
                return;
            }

            if (isFirstTime)
            {
                if (cardUI.popularityText != null)
                {
                    cardUI.popularityText.gameObject.SetActive(true);
                    cardUI.popularityText.text = originalValue.ToString();
                    _popularityPreviewAnimationQueue.Enqueue(Tween
                        .Scale(cardUI.popularityText.transform, Vector3.one, 0.5f).OnComplete(
                            () =>
                            {
                                cardUI.popularityText.text = finalValue.ToString();
                                if (finalValue != originalValue)
                                    Tween.PunchScale(cardUI.popularityText.transform, Vector3.one * 1.3f, 0.3f);
                            }));
                }

                if (cardUI.popularityImage != null)
                    cardUI.popularityImage.gameObject.SetActive(true);
                if (VBattle.Instance.BattleAttributeManager.MultiplierManager != null)
                    cardUI.SetPopularityImage(VBattle.Instance.BattleAttributeManager.MultiplierManager.Multiplier
                        .AttributeName);
            }
            else
            {
                if (cardUI.popularityText == null)
                    return;
                if (cardUI.popularityText.text == finalValue.ToString())
                    return;
                cardUI.popularityText.text = finalValue.ToString();
                _popularityPreviewAnimationQueue.Enqueue(Tween.PunchScale(cardUI.popularityText.transform,
                    Vector3.one * 1.3f, 0.3f));
            }
        }

        public void SetShieldPreview(bool isFirstTime, int originalValue, int finalValue)
        {
            if (originalValue == 0)
            {
                if (cardUI.shieldText != null)
                    cardUI.shieldText.gameObject.SetActive(false);
                if (cardUI.shieldImage != null)
                    cardUI.shieldImage.gameObject.SetActive(false);
                return;
            }

            if (isFirstTime)
            {
                if (cardUI.shieldText != null)
                {
                    cardUI.shieldText.gameObject.SetActive(true);
                    cardUI.shieldText.text = originalValue.ToString();
                    _shieldPreviewAnimationQueue.Enqueue(Tween.Scale(cardUI.shieldText.transform, Vector3.one, 0.5f)
                        .OnComplete(
                            () =>
                            {
                                cardUI.shieldText.text = finalValue.ToString();
                                if (finalValue != originalValue)
                                    Tween.PunchScale(cardUI.shieldText.transform, Vector3.one * 1.3f, 0.3f);
                            }));
                }

                if (cardUI.shieldImage != null)
                    cardUI.shieldImage.gameObject.SetActive(true);
            }
            else
            {
                if (cardUI.shieldText != null)
                {
                    if (cardUI.shieldText.text == finalValue.ToString())
                        return;
                    cardUI.shieldText.text = finalValue.ToString();
                    _shieldPreviewAnimationQueue.Enqueue(Tween.PunchScale(cardUI.shieldText.transform,
                        Vector3.one * 1.3f, 0.3f));
                }
            }
        }
    }
}