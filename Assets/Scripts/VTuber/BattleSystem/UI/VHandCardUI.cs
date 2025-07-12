using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
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
        }
        
        public void ToHandSlot(Vector3 position, Vector3 rotation, Vector3 scale, float smoothTime)
        {
            SetPosition(position, smoothTime, true, () => SetInteractive(true));
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
        
        public void SetCardUnusable()
        {
            SetInteractive(false);
            cardUI.background.color = Color.gray;
        }
        
        public void SetCardUsable()
        {
            SetInteractive(true);
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
            CardMovement();
            DetectDeselect();
        }
        
        private void Play()
        {
            Deselect();
            SetInteractive(false);
            card.Play();
        }
        
        private void CardMovement()
        {
            // if (_isMoving)
            // {
            //
            //     if (Mathf.Abs(transform.localPosition.x - _targetPosition.x) < 0.1f
            //         && Mathf.Abs(transform.localPosition.y - _targetPosition.y) < 0.01f)
            //     {
            //         _isMoving = false;
            //     }
            // }
            //
            // if (_isScaling)
            // {
            //     transform.localScale = Vector3.SmoothDamp(transform.localScale, _targetScale, ref _scaleVelocity, _scaleSmoothTime);
            //     if (Mathf.Abs(transform.localScale.x - _targetScale.x) < 0.01f
            //         && Mathf.Abs(transform.localScale.y - _targetScale.y) < 0.01f)
            //         _isScaling = false;
            // }
            //
            // if (_isRotating)
            // {
            //     _deltaTime += Time.deltaTime;
            //     if (Mathf.Abs(transform.localEulerAngles.z - _targetRotation.z) < 0.01f)
            //         _isRotating = false;
            // }
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
            if (!_inspectable)
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
            if (!_inspectable)
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
    }
}