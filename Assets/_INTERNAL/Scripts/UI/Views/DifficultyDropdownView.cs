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
        [SerializeField] private Vector2 _openState;
        [SerializeField] private Vector2 _closeState;
        [SerializeField] private float _toggleAnimationDuration = 0.25f;

        public event Action<Difficulty> OnDifficultySelected;

        private void Awake()
        {
            if(_openButton == null)
                _openButton = GetComponent<Button>();

            _panelRect = _panel.GetComponent<RectTransform>();

            _openState = new(_panelRect.sizeDelta.x, _panelRect.sizeDelta.y);
            _closeState = new(_panelRect.sizeDelta.x, 0f);

            _openButton.onClick.AddListener(Open);

            if (_panel.activeSelf)
            {
                _panel.SetActive(false);
                _panelRect.sizeDelta = _closeState;
            }
        }

        private void Open()
        {
            _panel.SetActive(true);

            _panelRect
                .DOSizeDelta(_openState, _toggleAnimationDuration)
                .SetEase(Ease.InSine);

            _openButtonIcon.sprite = _closeSprite;
        }

        private void Close()
        {
            
            _panelRect
                .DOSizeDelta(_closeState, _toggleAnimationDuration)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    _panel.SetActive(false);
                });

            _openButtonIcon.sprite = _openSprite;
        }

        private void OnDestroy()
        {
            _openButton.onClick.RemoveListener(Open);
        }

        public void Select(Difficulty difficulty)
        {
            _label.text = difficulty.ToString();

            OnDifficultySelected?.Invoke(difficulty);

            Close();
        }
    }
}