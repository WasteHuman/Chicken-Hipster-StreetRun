using DG.Tweening;
using UnityEngine;

namespace UI.Animations
{
    public class ButtonAnimations : MonoBehaviour
    {
        [Header("Animation Setup")]
        [SerializeField] private float _pulseAnimationDuration = 0.5f;
        [SerializeField] private bool _isAnimationEnabled = true;
        [SerializeField] private Vector3 _targetScale = Vector3.one;

        private RectTransform _rectTransform;

        private Tween _pulseTween;

        private void Awake()
        {
            _rectTransform = GetComponent <RectTransform>();

            if (_isAnimationEnabled)
                PulseAnimation();
        }

        private void PulseAnimation()
        {
            _pulseTween?.Kill();

            _pulseTween = _rectTransform
                .DOScale(_targetScale, _pulseAnimationDuration)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}