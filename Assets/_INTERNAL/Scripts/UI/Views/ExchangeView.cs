using Core.Gameplay;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Views.AddPayoutMethod;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class ExchangeView : MonoBehaviour
    {
        [Header("Coins (swap to USDT)")]
        [SerializeField] private Slider _coinsSlider;
        [SerializeField] private TextMeshProUGUI _coinsBalanceLabel;
        [SerializeField] private TextMeshProUGUI _coinsSelectedLabel;
        [SerializeField] private TextMeshProUGUI _coinsToUSDTLabel;

        [Header("USDT (withdraw)")]
        [SerializeField] private Slider _usdtSlider;
        [SerializeField] private TextMeshProUGUI _usdtBalanceLabel;
        [SerializeField] private TextMeshProUGUI _usdtBalanceToExchangeLabel;
        [SerializeField] private TextMeshProUGUI _usdtSelectedLabel;
        [SerializeField] private Button _exchangeUSDTButton;

        [Space(5), Header("Play To Start/Swap To USDT Button Setup")]
        [SerializeField] private TextMeshProUGUI _coinsActionButtonLabel;
        [SerializeField] private Button _coinsActionButton;

        [Space(5), Header("Fixed Swap Button (10k -> 5 USDT)")]
        [SerializeField] private Button _fixedSwapButton;

        [Space(5), Header("Connected Payout Methods")]
        [SerializeField] private List<ExchangePayoutItemView> _connectedMethods = new();
        [SerializeField] private ExchangePayoutItemView _exchangePayoutViewPrefab;
        [SerializeField] private RectTransform _connectedMethodsContainer;

        public event Action<float> OnCoinsSliderChanged;
        public event Action OnCoinsActionButtonClicked;
        public event Action<float> OnUSDTSliderChanged;
        public event Action OnExchangeUSDTClicked;
        public event Action OnFixedSwapClicked;

        private void Awake()
        {
            if (_coinsSlider != null)
                _coinsSlider.onValueChanged.AddListener(HandleCoinsSliderChanged);

            if (_coinsActionButton != null)
                _coinsActionButton.onClick.AddListener(() => OnCoinsActionButtonClicked?.Invoke());

            if (_usdtSlider != null)
                _usdtSlider.onValueChanged.AddListener(HandleUSDTSliderChanged);

            if (_exchangeUSDTButton != null)
                _exchangeUSDTButton.onClick.AddListener(() => OnExchangeUSDTClicked?.Invoke());

            if (_fixedSwapButton != null)
                _fixedSwapButton.onClick.AddListener(() => OnFixedSwapClicked?.Invoke());

            _coinsSelectedLabel.text = $"{1f}";
        }

        private void OnDestroy()
        {
            if (_coinsSlider != null)
                _coinsSlider.onValueChanged.RemoveListener(HandleCoinsSliderChanged);

            if (_coinsActionButton != null)
                _coinsActionButton.onClick.RemoveAllListeners();

            if (_usdtSlider != null)
                _usdtSlider.onValueChanged.RemoveListener(HandleUSDTSliderChanged);

            if (_exchangeUSDTButton != null)
                _exchangeUSDTButton.onClick.RemoveAllListeners();

            if (_fixedSwapButton != null)
                _fixedSwapButton.onClick.RemoveAllListeners();
        }

        public void LoadConnectedPayoutElements(IReadOnlyList<ExchangePayoutItemView> payoutItemViews)
        {
            foreach (var element in payoutItemViews)
            {
                var newPayoutElement = Instantiate(_exchangePayoutViewPrefab, _connectedMethodsContainer);
                newPayoutElement.SetupIcon(element.Icon);
                _connectedMethods.Add(newPayoutElement);
            }
        }

        public void AddPayoutMethods(Sprite icon)
        {
            if (icon == null)
                return;

            var newPayoutElement = Instantiate(_exchangePayoutViewPrefab, _connectedMethodsContainer);
            newPayoutElement.SetupIcon(icon);

            _connectedMethods.Add(newPayoutElement);
            EconomyController.Instance.AddNewPayoutMethod(newPayoutElement);
        }

        public void SetFixedSwapButtonInteractable(bool value) => _fixedSwapButton.interactable = value;

        public void SetCurrentCoinsBalance(float coins)
        {
            _coinsBalanceLabel.text = $"/{coins:N0}";
        }

        public void UpdateCoinsSelection(float coinsSelected)
        {
            _coinsSelectedLabel.text = $"{coinsSelected:N0}";
        }

        public void SetCoinsActionButtonText(string text)
        {
            if (_coinsActionButtonLabel != null)
                _coinsActionButtonLabel.text = text;
        }

        public float GetCoinsSliderPercent()
        {
            return _coinsSlider != null ? _coinsSlider.value : 0f;
        }

        public void SetCoinsSliderPercent(float percent)
        {
            if (_coinsSlider != null)
                _coinsSlider.value = Mathf.Clamp01(percent);
        }

        public void SetCurrentUSDTBalance(float usdt)
        {
            _usdtBalanceLabel.text = $"${usdt:F0}";
            if (_usdtSlider != null)
            {
                float min = 100f;
                float max = Mathf.Max(usdt, min);
                _usdtSlider.minValue = min;
                _usdtSlider.maxValue = max;
                _usdtSlider.value = Mathf.Clamp(_usdtSlider.value, min, max);
                _exchangeUSDTButton.interactable = usdt >= min;
                UpdateUSDTSelection(_usdtSlider != null ? _usdtSlider.value : 0f, max);
            }
        }

        public void UpdateUSDTSelection(float amount, float max = 0f)
        {
            _usdtSelectedLabel.text = $"${amount:F0}";
            _usdtBalanceToExchangeLabel.text = $"${max:F0}";
        }

        public float GetUSDTSliderValue()
        {
            return _usdtSlider != null ? _usdtSlider.value : 0f;
        }

        private void HandleCoinsSliderChanged(float val)
        {
            OnCoinsSliderChanged?.Invoke(val);
        }

        private void HandleUSDTSliderChanged(float val)
        {
            OnUSDTSliderChanged?.Invoke(val);
        }
    }
}