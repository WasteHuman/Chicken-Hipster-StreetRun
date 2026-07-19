using Core.Gameplay;
using System;
using UnityEngine;
using UnityEngine.UI;
using Screen = UI.Other.Screen;

namespace UI
{
    public class ScreensController : MonoBehaviour
    {
        private const string ShownLetsPlayKey = "ShownLetsPlay";
        private const string DailyLastClaimKey = "DailyBonusLastClaim";

        [Header("Screens")]
        [SerializeField] private Screen _letsPlayScreen;
        [SerializeField] private Screen _gameScreen;
        [SerializeField] private Screen _dailyBonusScreen;
        [SerializeField] private Screen _technicalScreen;
        [SerializeField] private Screen _popupsScreen;

        [Space(5), Header("Buttons")]
        [SerializeField] private Button _letsPlayButton;
        [SerializeField] private Button _collectDailyBonusButton;

        [Space(5), Header("Daily Bonus Settings")]
        [SerializeField] private int _dailyAmount = 1000;
        [SerializeField] private bool _debugDisableCooldown = false;

        public event Action OnGamePrepared;

        private void Awake()
        {
            _letsPlayButton.onClick.AddListener(HandleLetsPlayButtonClick);
            _collectDailyBonusButton.onClick.AddListener(HandleCollectDailyBonusButtonClick);
        }

        private void Start()
        {
            if (_gameScreen.IsActive)
            {
                if (PlayerPrefs.GetInt(ShownLetsPlayKey, 0) == 0)
                {
                    _gameScreen.Close();
                    _letsPlayScreen.Open();
                    PlayerPrefs.SetInt(ShownLetsPlayKey, 1);
                    PlayerPrefs.Save();
                }
                else
                    OnGamePrepared?.Invoke();
            }
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.DeleteKey(ShownLetsPlayKey);
        }

        private void OnDestroy()
        {
            _letsPlayButton.onClick.RemoveListener(HandleLetsPlayButtonClick);
            _collectDailyBonusButton.onClick.RemoveListener(HandleCollectDailyBonusButtonClick);
        }

        private void HandleLetsPlayButtonClick()
        {
            _letsPlayScreen.Close();

            if(IsDailyAvailable())
                _dailyBonusScreen.Open();
            else
            {
                _popupsScreen.Open();
                _gameScreen.Open();
                OnGamePrepared?.Invoke();
            }
        }

        private void HandleCollectDailyBonusButtonClick()
        {
            if (!IsDailyAvailable())
                return;

            try
            {
                EconomyController.Instance.AddCoins(_dailyAmount);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to add daily bonus: {e}");
            }

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            PlayerPrefs.SetString(DailyLastClaimKey, now.ToString());
            PlayerPrefs.Save();

            _collectDailyBonusButton.interactable = false;
            _dailyBonusScreen.Close();

            _popupsScreen.Open();
            _gameScreen.Open();
            OnGamePrepared?.Invoke();
        }

        private bool IsDailyAvailable()
        {
            if (_debugDisableCooldown)
                return true;

            long lastClaim = 0;
            var saved = PlayerPrefs.GetString(DailyLastClaimKey, "");
            if (!string.IsNullOrEmpty(saved))
            {
                long.TryParse(saved, out lastClaim);
            }

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long cooldown = GetCooldownSeconds();

            return (now - lastClaim) >= cooldown;
        }

        private long GetCooldownSeconds()
        {
            return 24 * 60 * 60;
        }
    }
}