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
                UpdateCoinsDisplay(EconomyController.Instance.GetCoinsBalance());
            }
        }

        private void OnDestroy()
        {
            if (EconomyController.Instance != null)
            {
                EconomyController.Instance.OnBalanceChanged -= UpdateCoinsDisplay;
            }
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