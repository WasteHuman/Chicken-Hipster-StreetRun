using DG.Tweening;
using Scripts.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [RequireComponent(typeof(Button))]
    public class DifficultyDropdownView : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Button _openButton;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private GameObject _panel;
        [SerializeField] private RectTransform _panelRect;

        [Space(5), Header("Open Button Sprites Setup")]
        [SerializeField] private Image _openButtonIcon;
        [SerializeField] private Sprite _openSprite;
        [SerializeField] private Sprite _closeSprite;

        [Space(5), Header("Animations Setup")]
        [SerializeField] private Vector2 _openPosition;
        [SerializeField] private Vector2 _closePosition;
        [SerializeField] private float _toggleAnimationDuration = 0.25f;

        private Tween _openTween;
        private Tween _closeTween;

        public event Action<Difficulty> OnDifficultySelected;

        private void Awake()
        {
            if(_openButton == null)
                _openButton = GetComponent<Button>();

            _closePosition = _panelRect.localPosition;

            _openButton.onClick.AddListener(Open);

            if (_panel.activeSelf)
                _panel.SetActive(false);
        }

        private void Open()
        {
            _openTween?.Kill();

            _panel.SetActive(true);

            _openTween = _panelRect
                .DOAnchorPos(_openPosition, _toggleAnimationDuration)
                .SetEase(Ease.OutBack);

            _openButtonIcon.sprite = _closeSprite;
        }

        private void Close()
        {
            _closeTween?.Kill();
            _openButton.interactable = false;

            _closeTween = _panelRect
                .DOAnchorPos(_closePosition, _toggleAnimationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _panel.SetActive(false);
                    _openButton.interactable = true;
                });

            _openButtonIcon.sprite = _openSprite;
        }

        private void OnDestroy()
        {
            _openButton.onClick.RemoveListener(Open);

            _openTween?.Kill();
            _closeTween?.Kill();
        }

        public void Select(Difficulty difficulty)
        {
            _label.text = difficulty.ToString();

            OnDifficultySelected?.Invoke(difficulty);

            Close();
        }
    }
}