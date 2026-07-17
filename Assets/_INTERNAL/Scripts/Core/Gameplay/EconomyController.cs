using System;
using UnityEngine;

namespace Core.Gameplay
{
    public class EconomyController : MonoBehaviour
    {
        private static EconomyController _instance;

        [SerializeField] private float _initialBalance = 1000f;

        private float _currentBalance;

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
            _currentBalance = _initialBalance;

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Получить текущий баланс
        /// </summary>
        public float GetBalance() => _currentBalance;

        /// <summary>
        /// Запросить текущий баланс (invoke события)
        /// </summary>
        public void RequestBalance() => OnBalanceChanged?.Invoke(_currentBalance);

        /// <summary>
        /// Добавить средства (выигрыш, бонус)
        /// </summary>
        public void Add(float amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"Попытка добавить отрицательную сумму: {amount}. Используйте метод Spend()");
                return;
            }

            _currentBalance += amount;
            OnBalanceChanged?.Invoke(_currentBalance);

            Debug.Log($"[Economy] Добавлено: +{amount}. Новый баланс: {_currentBalance}");
        }

        /// <summary>
        /// Списать средства (ставка, проигрыш)
        /// </summary>
        public bool Spend(float amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"Попытка списать отрицательную сумму: {amount}. Используйте метод Add()");
                return false;
            }

            if (_currentBalance < amount)
            {
                Debug.LogWarning($"Недостаточно средств! Баланс: {_currentBalance}, требуется: {amount}");
                return false;
            }

            _currentBalance -= amount;
            OnBalanceChanged?.Invoke(_currentBalance);

            Debug.Log($"[Economy] Списано: -{amount}. Новый баланс: {_currentBalance}");
            return true;
        }

        /// <summary>
        /// Проверить, достаточно ли средств
        /// </summary>
        public bool HasEnoughBalance(float amount) => _currentBalance >= amount;

        /// <summary>
        /// Установить баланс (для тестирования или загрузки из сохранений)
        /// </summary>
        public void SetBalance(float amount)
        {
            _currentBalance = Mathf.Max(0, amount);
            OnBalanceChanged?.Invoke(_currentBalance);
        }

        /// <summary>
        /// Сбросить баланс на начальное значение
        /// </summary>
        public void ResetBalance()
        {
            _currentBalance = _initialBalance;
            OnBalanceChanged?.Invoke(_currentBalance);
        }
    }
}