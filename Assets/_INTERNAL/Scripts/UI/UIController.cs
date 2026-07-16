using Core.Gameplay;
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
        public event Action OnBoostRewardClicked;

        private void Awake()
        {
            _playButton.onClick.AddListener(HandlePlayButtonClick);
            _goButton.onClick.AddListener(HandleGoButtonClick);
            _cashOutButton.onClick.AddListener(HandleCashOutButtonClick);

            _windowsController.OnRewardClaimed += HandleClaimedReward;
            _windowsController.OnBoostRewardClicked += HandleBoostRewardClicked;
        }

        private void Start()
        {
            EconomyController.Instance.OnBalanceChanged += HandleChangedBalance;
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(HandlePlayButtonClick);
            _goButton.onClick.RemoveListener(HandleGoButtonClick);
            _cashOutButton.onClick.RemoveListener(HandleCashOutButtonClick);

            _windowsController.OnRewardClaimed -= HandleClaimedReward;
            _windowsController.OnBoostRewardClicked -= HandleBoostRewardClicked;

            EconomyController.Instance.OnBalanceChanged -= HandleChangedBalance;
        }

        public void SetGoButtonInteractableState(bool value) => _goButton.interactable = value;
        public void UpdateCashOutText(float amount) => _cashOutText.text = $"CASH OUT {amount:N0} USDT";
        public void ShowVictory(float winAmount) => _windowsController.ShowVictory(winAmount);

        private void HandleBoostRewardClicked()
        {
            OnBoostRewardClicked?.Invoke();
        }

        private void HandlePlayButtonClick()
        {
            _windowsController.StartGame();
            OnGameStarted?.Invoke();
        }

        private void HandleGoButtonClick() => OnGoButtonClicked?.Invoke();
        private void HandleCashOutButtonClick() => OnCashOutClicked?.Invoke();
        private void HandleClaimedReward() => OnRewardClaimed?.Invoke();
        private void HandleChangedBalance(float balance)
        {
            if (balance > 0f)
                SetGoButtonInteractableState(true);

            if (balance <= 0f)
                _playButton.interactable = false;
        }
    }
}