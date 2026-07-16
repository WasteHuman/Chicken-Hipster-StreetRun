using System;
using UI.Other;
using UI.Views;
using UnityEngine;

namespace UI
{
    public class WindowsController : MonoBehaviour
    {
        [Header("Windows")]
        [SerializeField] private Window _gamePanelIdle;
        [SerializeField] private Window _gamePanelPlay;
        [SerializeField] private Window _settigsWindow;
        [SerializeField] private Window _winPopupWindow;

        [Space(5), Header("Controllers")]
        [SerializeField] private ButtonsController _buttonsController;

        public event Action OnRewardClaimed;
        public event Action OnBoostRewardClicked;

        private void Awake()
        {
            (_winPopupWindow as WinPanelPopup).OnClaimClicked += HandleRewardClaimed;
            (_winPopupWindow as WinPanelPopup).OnBoostRewardClicked += HandleBoostRewardClicked;

            _buttonsController.OnSettingsButtonClick += HandleSettingsButtonClick;
        }

        private void OnDestroy()
        {
            (_winPopupWindow as WinPanelPopup).OnClaimClicked -= HandleRewardClaimed;
            (_winPopupWindow as WinPanelPopup).OnBoostRewardClicked -= HandleBoostRewardClicked;

            _buttonsController.OnSettingsButtonClick -= HandleSettingsButtonClick;
        }

        public void StartGame()
        {
            _gamePanelIdle.Close();
            _gamePanelPlay.Open();
        }

        public void ShowVictory(float winAmount)
        {
            (_winPopupWindow as WinPanelPopup).SetWinAmount(winAmount);
            _winPopupWindow.Open();
        }

        public void RestartGame()
        {
            _gamePanelPlay.Close();
            _winPopupWindow.Close();
            _gamePanelIdle.Open();
        }

        private void HandleBoostRewardClicked()
        {
            OnBoostRewardClicked?.Invoke();
        }
        private void HandleSettingsButtonClick() => _settigsWindow.Open();
        private void HandleRewardClaimed()
        {
            OnRewardClaimed?.Invoke();
        }
    }
}