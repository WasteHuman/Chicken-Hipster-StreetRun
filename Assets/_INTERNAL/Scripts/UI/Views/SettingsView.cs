using DG.Tweening;
using UI.Other;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class SettingsView : Window
    {
        [Header("Buttons")]
        [SerializeField] private Button _closeSettingsButton;

        [Space(5), Header("Animation Setup")]
        [SerializeField] private float _toggleAnimationDuration = 0.25f;

        private RectTransform _rectTransform;

        private Tween _openTween;
        private Tween _closeTween;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            _closeSettingsButton.onClick.AddListener(Close);
        }

        private void OnDestroy()
        {
            _closeSettingsButton.onClick.RemoveListener(Close);
        }

        public override void Open()
        {
            gameObject.SetActive(true);

            _openTween?.Kill();

            _openTween = _rectTransform
                .DOScale(1f, _toggleAnimationDuration)
                .SetEase(Ease.InBack);
        }

        public override void Close()
        {
            _closeTween?.Kill();

            _closeTween = _rectTransform
                .DOScale(0f, _toggleAnimationDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }
    }
}