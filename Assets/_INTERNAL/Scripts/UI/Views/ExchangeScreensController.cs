using UnityEngine;
using UnityEngine.UI;
using Screen = UI.Other.Screen;

namespace UI.Views
{
    public class ExchangeScreensController : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private Screen _addToPayoutMethod;
        [SerializeField] private Screen _exchangeScreen;

        [Space(5), Header("Buttons")]
        [SerializeField] private Button _goToExchangeButton;
        [SerializeField] private Button _goToAddToPayoutMethodButton;

        private void Awake()
        {
            _goToExchangeButton.onClick.AddListener(HandleGoToExchangeButtonClick);
            _goToAddToPayoutMethodButton.onClick.AddListener(HandleGoToAddToPayoutButtonClick);
        }

        private void OnDestroy()
        {
            _goToExchangeButton.onClick.RemoveListener(HandleGoToExchangeButtonClick);
            _goToAddToPayoutMethodButton.onClick.RemoveListener(HandleGoToAddToPayoutButtonClick);
        }

        private void HandleGoToAddToPayoutButtonClick()
        {
            _addToPayoutMethod.Open();
            _exchangeScreen.Close();
        }

        private void HandleGoToExchangeButtonClick()
        {
            _addToPayoutMethod.Close();
            _exchangeScreen.Open();
        }
    }
}