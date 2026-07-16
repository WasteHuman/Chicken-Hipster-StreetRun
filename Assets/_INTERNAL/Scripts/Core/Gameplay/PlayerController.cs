using TMPro;
using UnityEngine;

namespace Core.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currentCoinsText;

        private void Start()
        {
            if (EconomyController.Instance != null)
            {
                EconomyController.Instance.OnBalanceChanged += UpdateCoinsDisplay;
                UpdateCoinsDisplay(EconomyController.Instance.GetBalance());
            }
        }

        private void OnDestroy()
        {
            if (EconomyController.Instance != null)
            {
                EconomyController.Instance.OnBalanceChanged -= UpdateCoinsDisplay;
            }
        }

        /// <summary>
        /// Добавить средства (выигрыш, бонус)
        /// </summary>
        public void Add(float amount)
        {
            EconomyController.Instance.Add(amount);
        }

        /// <summary>
        /// Списать средства (ставка, проигрыш)
        /// </summary>
        public bool Spend(float amount)
        {
            return EconomyController.Instance.Spend(amount);
        }

        /// <summary>
        /// Получить текущий баланс
        /// </summary>
        public float GetBalance()
        {
            return EconomyController.Instance.GetBalance();
        }

        private void UpdateCoinsDisplay(float balance)
        {
            if (_currentCoinsText != null)
            {
                _currentCoinsText.text = balance.ToString("N0");
            }
        }
    }
}