using System;
using System.Collections.Generic;
using UI.Views.AddPayoutMethod;
using UnityEngine;

namespace Core.Gameplay
{
    public class EconomyController : MonoBehaviour
    {
        private static EconomyController _instance;

        [SerializeField] private float _initialBalance = 100000f;

        private readonly List<ExchangePayoutItemView> _connectedPayoutMethods = new();

        private float _currentCoinsBalance;
        private float _currentUSDTBalance;

        public bool HasConnectedPayoutMethods => _connectedPayoutMethods.Count > 0;

        public event Action<float> OnUSDTBalanceChanged;
        public event Action<float> OnBalanceChanged;

        public static EconomyController Instance
        {
            get => _instance;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _currentCoinsBalance = _initialBalance;

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Получить текущий баланс Coins
        /// </summary>
        public float GetCoinsBalance() => _currentCoinsBalance;

        /// <summary>
        /// Получить текущий баланс USDT
        /// </summary>
        public float GetUSDTBalance() => _currentUSDTBalance;

        /// <summary>
        /// Запросить текущий баланс Coins (invoke события)
        /// </summary>
        public void RequestCoinsBalance() => OnBalanceChanged?.Invoke(_currentCoinsBalance);

        /// <summary>
        /// Запросить текущий баланс USDT (invoke события)
        /// </summary>
        public void RequestUSDTBalance() => OnUSDTBalanceChanged?.Invoke(_currentUSDTBalance);

        public IReadOnlyList<ExchangePayoutItemView> RequestConnectedPayoutMethods() => _connectedPayoutMethods.AsReadOnly();

        /// <summary>
        /// Добавить новый метод вывода
        /// </summary>
        public void AddNewPayoutMethod(ExchangePayoutItemView payoutItemView)
        {
            if (_connectedPayoutMethods.Contains(payoutItemView))
                return;

            _connectedPayoutMethods.Add(payoutItemView);
        }

        /// <summary>
        /// Добавить средства (выигрыш, бонус)
        /// </summary>
        public void AddCoins(float amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"Attempt to add a negattive amount: {amount}. Use the SpendCoins() method");
                return;
            }

            _currentCoinsBalance += amount;
            OnBalanceChanged?.Invoke(_currentCoinsBalance);

            Debug.Log($"[Economy] Added: +{amount}. New balance: {_currentCoinsBalance}");
        }

        /// <summary>
        /// Перевести монеты в USDT
        /// </summary>
        public void AddUSDT(float amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"Attempt to add a negattive amount: {amount}. Use the SpendCoins() method");
                return;
            }

            _currentUSDTBalance += amount;
            OnUSDTBalanceChanged?.Invoke(_currentUSDTBalance);

            Debug.Log($"[Economy] Added: +{amount}. New balance: {_currentUSDTBalance}");
        }

        /// <summary>
        /// Вывести USDT
        /// </summary>
        public bool SpendUSDT(float amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"Attempt to debit a negative amount: {amount}. Use the AddCoins() method");
                return false;
            }

            if (_currentUSDTBalance < amount)
            {
                Debug.LogWarning($"Not enough coins! Balance: {_currentUSDTBalance}, needed: {amount}");
                return false;
            }

            _currentUSDTBalance -= amount;
            OnUSDTBalanceChanged?.Invoke(_currentUSDTBalance);

            Debug.Log($"[Economy] Debited: -{amount}. New balance: {_currentUSDTBalance}");
            return true;
        }

        /// <summary>
        /// Списать средства (ставка, проигрыш)
        /// </summary>
        public bool SpendCoins(float amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"Attempt to debit a negative amount: {amount}. Use the AddCoins() method");
                return false;
            }

            if (_currentCoinsBalance < amount)
            {
                Debug.LogWarning($"Not enough coins! Balance: {_currentCoinsBalance}, needed: {amount}");
                return false;
            }

            _currentCoinsBalance -= amount;
            OnBalanceChanged?.Invoke(_currentCoinsBalance);

            Debug.Log($"[Economy] Debited: -{amount}. New balance: {_currentCoinsBalance}");
            return true;
        }

        /// <summary>
        /// Проверить, достаточно ли средств
        /// </summary>
        public bool HasEnoughBalance(float amount) => _currentCoinsBalance >= amount;

        /// <summary>
        /// Установить баланс (для тестирования или загрузки из сохранений)
        /// </summary>
        public void SetBalance(float amount)
        {
            _currentCoinsBalance = Mathf.Max(0, amount);
            OnBalanceChanged?.Invoke(_currentCoinsBalance);
        }

        /// <summary>
        /// Сбросить баланс на начальное значение
        /// </summary>
        public void ResetBalance()
        {
            _currentCoinsBalance = _initialBalance;
            OnBalanceChanged?.Invoke(_currentCoinsBalance);
        }
    }
}