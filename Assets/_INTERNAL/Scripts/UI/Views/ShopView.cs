using Core.Gameplay;
using Core.Services.AdsService;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private float _freeCoinsAmount = 1000f;
        [SerializeField] private Button _getFreemCoinsButton;

        private void Awake()
        {
            _getFreemCoinsButton.onClick.AddListener(HandleFreeCoinsButtonClick);
        }

        private void OnDestroy()
        {
            _getFreemCoinsButton.onClick.RemoveListener(HandleFreeCoinsButtonClick);
        }

        private void HandleFreeCoinsButtonClick()
        {
            AdsController.Instance.ShowRewardedAd(() =>
            {
                EconomyController.Instance.AddCoins(_freeCoinsAmount);
            });
        }
    }
}