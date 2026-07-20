using Core.Gameplay;
using System;
using UI.Views;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Exchange
{
    public class ExchangeController : MonoBehaviour
    {
        [SerializeField] private ExchangeView _view;

        private float _currentCoinsBalance = 0f;
        private float _currentUSDTBalance = 0f;
        private float _selectedCoins = 0f;

        private const float CoinsToUSDTRate = 5f / 10000f; // 10k coins = 5 USDT
        private const float MinCoinsToEnableSwap = 10000f;
        private const float MinUSDTWithdraw = 100f;
        private const float FixedCoinsAmount = 10000f;
        private const float FixedUSDTAmount = 5f;

        public float CurrentUSDTBalance
        {
            get => _currentUSDTBalance;
            private set
            {
                if (value < 0f)
                    throw new ArgumentOutOfRangeException(nameof(value), "New USDT balance can not be a negative!");

                _currentUSDTBalance = value;
            }
        }
        public float CurrentCoinsBalance
        {
            get => _currentCoinsBalance;
            private set
            {
                if (value < 0f)
                    throw new ArgumentOutOfRangeException(nameof(value), "New Coins balance can not be a negative!");

                _currentCoinsBalance = value;
            }
        }

        private void Start()
        {
            EconomyController.Instance.OnBalanceChanged += HandleChangedCoinsBalance;
            EconomyController.Instance.OnUSDTBalanceChanged += HandleChangedUSDTBalance;

            _view.OnCoinsSliderChanged += HandleCoinsSliderChanged;
            _view.OnCoinsActionButtonClicked += HandleCoinsActionButtonClicked;
            _view.OnUSDTSliderChanged += HandleUSDTSliderChanged;
            _view.OnExchangeUSDTClicked += HandleExchangeUSDTClicked;

            EconomyController.Instance.RequestCoinsBalance();
            EconomyController.Instance.RequestUSDTBalance();

            if (EconomyController.Instance.HasConnectedPayoutMethods)
                _view.LoadConnectedPayoutElements(EconomyController.Instance.RequestConnectedPayoutMethods());
        }

        private void OnDestroy()
        {
            if (EconomyController.Instance != null)
            {
                EconomyController.Instance.OnBalanceChanged -= HandleChangedCoinsBalance;
                EconomyController.Instance.OnUSDTBalanceChanged -= HandleChangedUSDTBalance;
            }

            if (_view != null)
            {
                _view.OnCoinsSliderChanged -= HandleCoinsSliderChanged;
                _view.OnCoinsActionButtonClicked -= HandleCoinsActionButtonClicked;
                _view.OnUSDTSliderChanged -= HandleUSDTSliderChanged;
                _view.OnExchangeUSDTClicked -= HandleExchangeUSDTClicked;
            }
        }

        public void AddPayoutMethod(Sprite icon) => _view.AddPayoutMethods(icon);

        private void FixedSwap()
        {
            if (!EconomyController.Instance.SpendCoins(FixedCoinsAmount))
            {
                Debug.LogWarning("[Exchange] Failed to debit coins for swap.");
                return;
            }

            EconomyController.Instance.AddUSDT(FixedUSDTAmount);
        }

        private void HandleChangedUSDTBalance(float usdt)
        {
            CurrentUSDTBalance = usdt;
            _view.SetCurrentUSDTBalance(CurrentUSDTBalance);
        }

        private void HandleChangedCoinsBalance(float coins)
        {
            CurrentCoinsBalance = coins;

            _view.SetCurrentCoinsBalance(CurrentCoinsBalance);
            _view.SetSwapCoinsToUSDTButtonInteractable(CurrentCoinsBalance >= MinCoinsToEnableSwap);
            _view.UpdateSwapCoinsToUSDTButtonLabel(_selectedCoins);
        }

        private void HandleCoinsSliderChanged(float percent)
        {
            float coinsSelected = Mathf.Clamp01(percent) * CurrentCoinsBalance;
            _selectedCoins = coinsSelected;

            _view.UpdateCoinsSelection(coinsSelected);
        }

        private void HandleCoinsActionButtonClicked()
        {
            float percent = _view.GetCoinsSliderPercent();
            float coinsToSwap = Mathf.Clamp01(percent) * CurrentCoinsBalance;
            if (coinsToSwap <= 0f)
            {
                FixedSwap();
                Debug.LogWarning($"[Exchange] No coins selected to swap. Fixed Swap: Coins: {FixedCoinsAmount:N1} / USDT: {FixedUSDTAmount:N0}");
                return;
            }

            if (!EconomyController.Instance.SpendCoins(coinsToSwap))
            {
                Debug.LogWarning("[Exchange] Failed to debit coins for swap.");
                return;
            }

            float usdtToAdd = coinsToSwap * CoinsToUSDTRate;
            EconomyController.Instance.AddUSDT(usdtToAdd);

            Debug.Log($"[Exchange] Swapped {coinsToSwap:N0} coins -> {usdtToAdd:F2} USDT");
            _view.SetCoinsSliderPercent(0f);
        }

        private void HandleUSDTSliderChanged(float amount)
        {
            _view.UpdateUSDTSelection(amount, CurrentUSDTBalance);
        }

        private void HandleExchangeUSDTClicked()
        {
            float amount = _view.GetUSDTSliderValue();
            if (amount < MinUSDTWithdraw)
            {
                Debug.LogWarning($"[Exchange] Minimum withdraw is {MinUSDTWithdraw} USDT.");
                return;
            }

            if (amount > CurrentUSDTBalance)
            {
                Debug.LogWarning($"[Exchange] Not enough USDT balance. Have {CurrentUSDTBalance}, requested {amount}");
                return;
            }

            bool success = EconomyController.Instance.SpendUSDT(amount);
            if (!success)
            {
                Debug.LogWarning("[Exchange] Failed to withdraw USDT.");
                return;
            }

            Debug.Log($"[Exchange] Withdrawn {amount} USDT");
        }
    }
}