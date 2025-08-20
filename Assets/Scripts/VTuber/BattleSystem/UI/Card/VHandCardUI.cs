using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VHandCardUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public VCard card;
        public VCardUI cardUI;
        public VBattleUI battleUI;
        
        private bool _isMoving = false;
        
        private Vector3 _targetPosition;
        private Vector3 _positionVelocity;
        private float _positionSmoothTime;
        
        private Vector3 _targetScale;
        private Vector3 _scaleVelocity;
        private float _scaleSmoothTime;
        
        private float _rotationVelocity;
        private float _rotationSmoothTime;
        private float _deltaTime;
        
        public int index;

        private bool _inspectable;
        private bool _isPlayable = true;

        [Header("Inspection")] 
        private Vector3 _inspectionScale;
        public float inspectionY = 150.0f;

        public Vector3 OriginalScale => _originalScale;
        private Vector3 _originalScale;

        public Vector3 OriginalPosition => _originalPosition;
        private Vector3 _originalPosition;

        public Vector3 OriginalRotation => _originalRotation;
        private Vector3 _originalRotation;
        
        private int _originalSiblingIndex;
        private bool _isPointerStaying = false;

        private bool _moveWithMouse;

        public bool selected;
        public bool selfSelected;

        private bool doubleCheck;

        private Dictionary<string, object> message;

        private bool selectClickUp = false;
        private Vector3 _targetRotation;
        private bool _isScaling;
        private bool _isRotating;
        private bool shouldWaitSetPlayable = false;
        
        private VAnimationQueue _popularityPreviewAnimationQueue;

        public VHandCardUI(Vector3 targetRotation, bool isScaling)
        {
            _targetRotation = targetRotation;
            _isScaling = isScaling;
        }


        protected override void Awake()
        {
            base.Awake();
            SetInteractive(false);
            _inspectionScale = new Vector3(1.0f, 1.0f, 1.0f);
            _originalSiblingIndex = transform.GetSiblingIndex();
            message = new Dictionary<string, object>();
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            _popularityPreviewAnimationQueue = new VAnimationQueue();
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
            Tween.Position(transform, targetPosition, smoothTime, Ease.Linear).OnComplete(DestroyGameObject);
            Tween.Scale(transform, Vector3.zero, smoothTime, Ease.Linear).OnComplete(DestroyGameObject);
        }

        private void DestroyGameObject()
        {
            if(gameObject)
                Destroy(gameObject);
        }
        
        public void SetCardPlayable(bool isPlayable)
        {
            if (!isPlayable)
            {
                _isPlayable = false;
                cardUI.background.color = Color.gray;
                return;
            }

            if (battleUI.IsCardApplying)
            {
                shouldWaitSetPlayable = true;
                return;
            }
            
            _isPlayable = true;
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
                ()=>
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
                _originalScale = _targetScale;
            Tween.Scale(transform, targetScale, smoothTime, Ease.Linear).OnComplete(
                ()=>
                {
                    _isScaling = false;
                });;
        }
        
        public void SetRotation(Vector3 targetAngle, float smoothTime, bool setOriginal)
        {
            _isRotating = true;
            _targetRotation = targetAngle;
            _rotationSmoothTime = smoothTime;
            if (setOriginal)
                _originalRotation = _targetRotation;
            Tween.LocalRotation(transform, targetAngle, smoothTime, Ease.Linear).OnComplete(
                ()=>
                {
                    _isRotating = false;
                });;
        }

        private void Update()
        {
            DetectDeselect();
        }
        
        private void Play()
        {
            Deselect();
            SetInteractive(false);
            card.Play();
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
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_inspectable || !_isPlayable)
                return;
            if (eventData.button == PointerEventData.InputButton.Left && !selected)
                Select();
            else if(selfSelected && eventData.button == PointerEventData.InputButton.Left)
                Play();
            
            if (selected && !selfSelected)
            {
                battleUI.UnselectCurrent();
                Select();
            }
        }

        public void OnCardStopApplying()
        {
            if (shouldWaitSetPlayable)
            {
                SetCardPlayable(true);
            }
        }

        public void SetPopularityPreview(bool isFirstTime, int originalValue, int finalValue)
        {
            if (originalValue == 0)
            {
                cardUI.popularityText.gameObject.SetActive(false);
                cardUI.popularityImage.gameObject.SetActive(false);
                return;
            }

            if (isFirstTime)
            {
                cardUI.popularityText.gameObject.SetActive(true);
                cardUI.popularityImage.gameObject.SetActive(true);
                if(VBattle.Instance.BattleAttributeManager.MultiplierManager is not null)
                    cardUI.SetPopularityImage(VBattle.Instance.BattleAttributeManager.MultiplierManager.Multiplier.AttributeName);
                cardUI.popularityText.text = originalValue.ToString();
                _popularityPreviewAnimationQueue.Enqueue(Tween.Scale(cardUI.popularityText.transform, Vector3.one, 0.5f).OnComplete(
                    () =>
                    {
                        cardUI.popularityText.text = finalValue.ToString();
                        if(finalValue != originalValue)
                            Tween.PunchScale(cardUI.popularityText.transform, Vector3.one * 1.3f, 0.3f);
                    }));
            }
            else
            {
                if (cardUI.popularityText.text == finalValue.ToString())
                    return;
                cardUI.popularityText.text = finalValue.ToString();
                _popularityPreviewAnimationQueue.Enqueue(Tween.PunchScale(cardUI.popularityText.transform, Vector3.one * 1.3f, 0.3f));
            }
        }
    }
}