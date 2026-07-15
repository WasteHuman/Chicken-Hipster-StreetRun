using System;
using UnityEngine;
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
            _settingsButton.onClick.AddListener(HandleSettingsButtonClick);
        }

        private void OnDestroy()
        {
            _settingsButton.onClick.RemoveListener(HandleSettingsButtonClick);
        }

        private void HandleSettingsButtonClick() => OnSettingsButtonClick?.Invoke();
    }
}