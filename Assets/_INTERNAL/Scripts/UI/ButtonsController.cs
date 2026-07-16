using Core;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class ButtonsController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _withdrawButton;
        [SerializeField] private Button _wheelOfLuckButton;
        [SerializeField] private Button _settingsButton;

        public event Action OnSettingsButtonClick;

        private void Awake()
        {
            _shopButton.onClick.AddListener(HandleShopButtonClick);
            _withdrawButton.onClick.AddListener(HandleWithdrawButtonClick);
            _wheelOfLuckButton.onClick.AddListener(HandleWheelOfLuckButtonClick);
            _settingsButton.onClick.AddListener(HandleSettingsButtonClick);
        }

        private void OnDestroy()
        {
            _shopButton.onClick.RemoveListener(HandleShopButtonClick);
            _withdrawButton.onClick.RemoveListener(HandleWithdrawButtonClick);
            _wheelOfLuckButton.onClick.RemoveListener(HandleWheelOfLuckButtonClick);
            _settingsButton.onClick.RemoveListener(HandleSettingsButtonClick);
        }

        private void HandleShopButtonClick() => SceneManager.LoadSceneAsync(SceneNames.SHOP);
        private void HandleWithdrawButtonClick() => SceneManager.LoadSceneAsync(SceneNames.WITHDRAW);
        private void HandleWheelOfLuckButtonClick() => SceneManager.LoadSceneAsync(SceneNames.WHEEL_OF_LUCK);
        private void HandleSettingsButtonClick() => OnSettingsButtonClick?.Invoke();
    }
}