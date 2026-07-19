using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.AddPayoutMethod
{
    public class PayoutItemWindowView : MonoBehaviour
    {
        [SerializeField] private GameObject _payoutWindow;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _payoutNameLabel;
        [SerializeField] private TMP_InputField _firstInputField;
        [SerializeField] private TMP_InputField _secondInputField;

        [Space(5), Header("Animation Setup")]
        [SerializeField] private float _closeAnimationDuration = 0.5f;
        [SerializeField] private float _openAnimationDuration = 0.5f;

        private string _firstInputResult;
        private string _secondInputResult;

        private RectTransform _rect;

        private Tween _closeTween;
        private Tween _openTween;

        public string FirstInputResult => _firstInputResult;
        public string SecondInputResult => _secondInputResult;

        public event Action OnSaveButtonClicked;

        public void Initialize()
        {
            _closeButton.onClick.AddListener(HandleCloseButtonClick);
            _saveButton.onClick.AddListener(HandleSaveButtonClick);

            _rect = _payoutWindow.GetComponent<RectTransform>();
            _rect.localScale = Vector2.zero;

            _firstInputField.onEndEdit.AddListener(HandleFirstEndInputEdit);
            _secondInputField.onEndEdit.AddListener(HandleSecondEndInputEdit);
        }

        public void Dispose()
        {
            _firstInputField.onEndEdit.RemoveListener(HandleFirstEndInputEdit);
            _secondInputField.onEndEdit.RemoveListener(HandleSecondEndInputEdit);

            _closeButton.onClick.RemoveListener(HandleCloseButtonClick);
            _saveButton.onClick.RemoveListener(HandleSaveButtonClick);

            _closeTween?.Kill();
            _openTween?.Kill();
        }

        public void ResetInputFields()
        {
            _firstInputResult = null;
            _secondInputResult = null;

            _firstInputField.text = string.Empty;
            _secondInputField.text = string.Empty;
        }

        public void SetupPayoutName(string name) => _payoutNameLabel.text = name;
        public void SetupIcon(Sprite icon) => _icon.sprite = icon;

        public void SetupInputFields(bool isSecondFieldNeeded)
        {
            if (isSecondFieldNeeded)
                _secondInputField.gameObject.SetActive(true);
            else
                _secondInputField.gameObject.SetActive(false);
        }

        public void Open()
        {
            if (!string.IsNullOrEmpty(_firstInputResult))
                _firstInputField.SetTextWithoutNotify(_firstInputResult);

            if (!string.IsNullOrEmpty(_secondInputResult))
                _secondInputField.SetTextWithoutNotify(_secondInputResult);

            PlayOpenAnimation();
        }

        private void PlayOpenAnimation()
        {
            _rect.localScale = Vector2.zero;
            gameObject.SetActive(true);

            _openTween?.Kill();

            _openTween = _rect
                .DOScale(Vector2.one, _openAnimationDuration)
                .SetEase(Ease.OutSine);
        }

        private void PlayCloseAnimation()
        {
            _closeTween?.Kill();

            _closeTween = _rect
                .DOScale(Vector2.zero, _closeAnimationDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => gameObject.SetActive(false));
        }

        private void HandleFirstEndInputEdit(string result) => _firstInputResult = result;
        private void HandleSecondEndInputEdit(string result) => _secondInputResult = result;

        private void HandleSaveButtonClick()
        {
            if (string.IsNullOrEmpty(_firstInputResult))
                return;

            if (_secondInputField.isActiveAndEnabled && string.IsNullOrEmpty(_secondInputResult))
                return;

            PlayCloseAnimation();
            OnSaveButtonClicked?.Invoke();
        }

        private void HandleCloseButtonClick()
        {
            PlayCloseAnimation();
        }
    }
}