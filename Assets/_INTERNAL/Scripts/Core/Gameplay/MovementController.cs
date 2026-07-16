using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Gameplay
{
    public class MovementController : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private RectTransform _movableRoot;
        [SerializeField] private List<Vector2> _stepAnchoredPositions = new();
        [SerializeField] private float _stepDuration = 0.45f;
        [SerializeField] private Ease _ease = Ease.OutQuad;

        private int _currentStepIndex = 0;

        private Tween _moveTween;

        public bool IsMoving { get; private set; }
        public bool CanMoveNextStep => _currentStepIndex <= _stepAnchoredPositions.Count - 1;
        public int CurrentStepIndex => _currentStepIndex;

        public event Action OnStepCompleted;

        private void Awake()
        {
            if (_stepAnchoredPositions.Count == 0)
            {
                Debug.LogError($"{nameof(MovementController)}: Step positions are not configured.", this);
                enabled = false;
            }
        }

        private void OnDestroy()
        {
            _moveTween?.Kill();
        }

        public void MoveStep()
        {
            if (IsMoving)
                return;

            if (!CanMoveNextStep)
                return;

            _moveTween?.Kill();

            Vector2 targetPosition = _stepAnchoredPositions[_currentStepIndex];

            _moveTween = _movableRoot
                .DOAnchorPos(targetPosition, _stepDuration)
                .SetEase(_ease)
                .OnComplete(() =>
                {
                    IsMoving = false;

                    OnStepCompleted?.Invoke();

                    _currentStepIndex++;
                });
        }

        public void ResetMovement()
        {
            _moveTween?.Kill();

            IsMoving = false;
            _currentStepIndex = 0;

            Vector2 position = _movableRoot.anchoredPosition;
            position.x = 0f;
            _movableRoot.anchoredPosition = position;
        }
    }
}