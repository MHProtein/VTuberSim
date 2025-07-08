using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VHandCardUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public VCard card;
        public VBattleUI battleUI;
        
        private bool _isMoving = false;
        
        public Vector3 TargetPosition => _targetPosition;
        private Vector3 _targetPosition;
        private Vector3 _positionVelocity;
        private float _positionSmoothTime;

        private bool _isScaling = false;
        
        public Vector3 TargetScale => _targetScale;
        private Vector3 _targetScale;
        private Vector3 _scaleVelocity;
        private float _scaleSmoothTime;
    
        private bool _isRotating = false;
        
        public Vector3 TargetRotation => _targetRotation;
        private Vector3 _targetRotation;
        private float _rotationVelocity;
        private float _rotationSmoothTime;
        private float _deltaTime;
        
        public int index;

        [Header("Inspection")] 
        public Vector3 inspectionScale = new Vector3(1.5f, 1.5f, 1.0f);
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

        
        protected override void Awake()
        {
            base.Awake();
            _originalSiblingIndex = transform.GetSiblingIndex();
            message = new Dictionary<string, object>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }     
        
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        
        public void SetPosition(Vector3 targetPosition, float smoothTime, bool setOriginal)
        {
            _isMoving = true;
            _targetPosition = targetPosition;
            _positionSmoothTime = smoothTime;
            if (setOriginal)
                _originalPosition = targetPosition;
        }
        
        public void SetOriginalPosition(float smoothTime)
        {
            _isMoving = true;
            _targetPosition = _originalPosition;
            _positionSmoothTime = smoothTime;
        }
        
        public void SetScale(Vector3 targetScale, float smoothTime, bool setOriginal)
        {
            _isScaling = true;
            _targetScale = targetScale;
            _scaleSmoothTime = smoothTime;
            if (setOriginal)
                _originalScale = _targetScale;
        }
        
        public void SetRotation(Vector3 targetAngle, float smoothTime, bool setOriginal)
        {
            _isRotating = true;
            _targetRotation = targetAngle;
            _rotationSmoothTime = smoothTime;
            if (setOriginal)
                _originalRotation = _targetRotation;
        }

        private void Update()
        {
            CardMovement();
            DetectDeselect();
            Play();
        }
        
        private void Play()
        {
            if (selfSelected && Input.GetMouseButtonUp(0))
                selectClickUp = true;
            if (selectClickUp && Input.GetMouseButtonDown(0))
            {
                Deselect();
                card.Play();
                battleUI.Rearrange(index);
                Destroy(gameObject);
            }
        }
        
        private void CardMovement()
        {
            if (_isMoving)
            {
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, _targetPosition, ref _positionVelocity, _positionSmoothTime);
                if (Mathf.Abs(transform.localPosition.x - _targetPosition.x) < 0.01f
                    && Mathf.Abs(transform.localPosition.y - _targetPosition.y) < 0.01f)
                {
                    _isMoving = false;
                }
            }
            
            if (_isScaling)
            {
                transform.localScale = Vector3.SmoothDamp(transform.localScale, _targetScale, ref _scaleVelocity, _scaleSmoothTime);
                if (Mathf.Abs(transform.localScale.x - _targetScale.x) < 0.01f
                    && Mathf.Abs(transform.localScale.y - _targetScale.y) < 0.01f)
                    _isScaling = false;
            }

            if (_isRotating)
            {
                _deltaTime += Time.deltaTime;
                transform.localEulerAngles = Vector3.Slerp(transform.localEulerAngles,
                    _targetRotation, _deltaTime / _rotationSmoothTime);
                if (Mathf.Abs(transform.localEulerAngles.z - _targetRotation.z) < 0.01f)
                    _isRotating = false;
            }

            if (_moveWithMouse)
            {
                transform.position = Input.mousePosition;
            }
            
            DetectDeselect();
        }
        
        private void DetectDeselect()
        {
            if (!selfSelected)
                return;

            if (!Input.GetMouseButtonDown(1))
                return;

            Deselect();
        }

        private void Deselect()
        {
            selfSelected = false;
            selectClickUp = false;
            battleUI.Selected(false);
            ExitInspection();
        }
        
        private void Inspect()
        {
            var pos = new Vector3(_originalPosition.x, inspectionY, _originalPosition.z);
            SetPosition(pos, _positionSmoothTime, false);

            SetRotation(Vector3.zero, _rotationSmoothTime, false);

            SetScale(inspectionScale, _scaleSmoothTime, false);
            transform.SetAsLastSibling();
        }

        private void ExitInspection()
        {
            SetPosition(_originalPosition, _positionSmoothTime, false);

            SetRotation(_originalRotation, _rotationSmoothTime, false);

            SetScale(_originalScale, _scaleSmoothTime, false);
            transform.SetSiblingIndex(_originalSiblingIndex);
        }

        private void Select()
        {
            selected = true;
            selfSelected = true;
            battleUI.Selected(true);
            VDebug.Log($"Card {card.CardName} selected at index {index}");
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerStaying = true;
        
            if (selected)
                return;
            Inspect();
            //battleUI.MoveAway(index);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (selected)
            {
                return;
            }
            _isPointerStaying = false;
            ExitInspection();
            //battleUI.MoveBack(index);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && !selected)
                Select();
        }
    }
}