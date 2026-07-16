using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _goButton;
        [SerializeField] private Button _cashOutButton;

        [Space(5), Header("Text")]
        [SerializeField] private TextMeshProUGUI _cashOutText;

        [Space(5), Header("Mini-Controllers")]
        [SerializeField] private WindowsController _windowsController;

        public event Action OnGoButtonClicked;
        public event Action OnCashOutClicked;
        public event Action OnGameStarted;
        public event Action OnRewardClaimed;

        private void Awake()
        {
            _playButton.onClick.AddListener(HandlePlayButtonClick);
            _goButton.onClick.AddListener(HandleGoButtonClick);
            _cashOutButton.onClick.AddListener(HandleCashOutButtonClick);

            _windowsController.OnRewardClaimed += HandleClaimedReward;
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(HandlePlayButtonClick);
            _goButton.onClick.RemoveListener(HandleGoButtonClick);
            _cashOutButton.onClick.RemoveListener(HandleCashOutButtonClick);

            _windowsController.OnRewardClaimed -= HandleClaimedReward;
        }

        public void RestartGame() => _windowsController.RestartGame();
        public void SetGoButtonInteractableState(bool value) => _goButton.interactable = value;
        public void UpdateCashOutText(float amount) => _cashOutText.text = $"CASH OUT {amount:N0} USDT";
        public void ShowVictory(float winAmount) => _windowsController.ShowVictory(winAmount);

        private void HandlePlayButtonClick()
        {
            _windowsController.StartGame();
            OnGameStarted?.Invoke();
        }

        private void HandleGoButtonClick() => OnGoButtonClicked?.Invoke();
        private void HandleCashOutButtonClick() => OnCashOutClicked?.Invoke();
        private void HandleClaimedReward()
        {
            OnRewardClaimed?.Invoke();
        }
    }
}