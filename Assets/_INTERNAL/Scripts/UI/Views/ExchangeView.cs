using Core;
using Core.Gameplay;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Views.AddPayoutMethod;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private Button _coinsActionButton;

        [Space(5), Header("Buttons")]
        [SerializeField] private Button _swapCoinsToUSDTButton;
        [SerializeField] private Button _playToStartButton;

        [Space(5), Header("Connected Payout Methods")]
        [SerializeField] private List<ExchangePayoutItemView> _connectedMethods = new();
        [SerializeField] private ExchangePayoutItemView _exchangePayoutViewPrefab;
        [SerializeField] private RectTransform _connectedMethodsContainer;

        public event Action<float> OnCoinsSliderChanged;
        public event Action OnCoinsActionButtonClicked;
        public event Action<float> OnUSDTSliderChanged;
        public event Action OnExchangeUSDTClicked;

        private void Awake()
        {
            if (_coinsSlider != null)
                _coinsSlider.onValueChanged.AddListener(HandleCoinsSliderChanged);

            if (_swapCoinsToUSDTButton != null)
                _swapCoinsToUSDTButton.onClick.AddListener(() => OnCoinsActionButtonClicked?.Invoke());

            if (_usdtSlider != null)
                _usdtSlider.onValueChanged.AddListener(HandleUSDTSliderChanged);

            if (_exchangeUSDTButton != null)
                _exchangeUSDTButton.onClick.AddListener(() => OnExchangeUSDTClicked?.Invoke());

            if (_playToStartButton != null)
                _playToStartButton.onClick.AddListener(HandlePlayToStartButtonClick);

            _coinsSelectedLabel.text = $"{1f}";

            UpdateSwapCoinsToUSDTButtonLabel(0f);
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

            if (_swapCoinsToUSDTButton != null)
                _swapCoinsToUSDTButton.onClick.RemoveAllListeners();

            if (_playToStartButton != null)
                _playToStartButton.onClick.RemoveListener(HandlePlayToStartButtonClick);
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

        public void SetSwapCoinsToUSDTButtonInteractable(bool value) => _swapCoinsToUSDTButton.interactable = value;

        public void UpdateSwapCoinsToUSDTButtonLabel(float selectedCoins)
        {
            const float MIN_THRESHOLD = 10000f;
            const float MIN_USDT = 5f;

            if (selectedCoins < MIN_THRESHOLD)
                _coinsToUSDTLabel.text = $"{MIN_THRESHOLD:N0} -> ${MIN_USDT:N0}";
            else
            {
                float usdtValue = selectedCoins * (MIN_USDT / MIN_THRESHOLD);
                _coinsToUSDTLabel.text = $"{selectedCoins:N0} -> ${usdtValue:N0}";
            }

            Debug.Log($"[Exchange View] Button Label: {_coinsToUSDTLabel.text} (selected: {selectedCoins:N0})");
        }

        public void SetCurrentCoinsBalance(float coins)
        {
            _coinsBalanceLabel.text = $"/{coins:N0}";
        }

        public void UpdateCoinsSelection(float coinsSelected)
        {
            _coinsSelectedLabel.text = $"{coinsSelected:N0}";
            UpdateSwapCoinsToUSDTButtonLabel(coinsSelected);
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
            _usdtBalanceLabel.text = $"${usdt:N0}";
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
            _usdtSelectedLabel.text = $"${amount:N0}";
            _usdtBalanceToExchangeLabel.text = $"${max:N0}";
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

        private void HandlePlayToStartButtonClick() => SceneManager.LoadSceneAsync(SceneNames.MAIN);
    }
}