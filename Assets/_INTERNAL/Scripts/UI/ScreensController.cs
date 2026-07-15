using UnityEngine;
using UnityEngine.UI;

using Screen = UI.Other.Screen;

namespace UI
{
    public class ScreensController : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private Screen _letsPlayScreen;
        [SerializeField] private Screen _gameScreen;
        [SerializeField] private Screen _dailyBonusScreen;
        [SerializeField] private Screen _technicalScreen;
        [SerializeField] private Screen _popupsScreen;

        [Space(5), Header("Buttons")]
        [SerializeField] private Button _letsPlayButton;
        [SerializeField] private Button _collectDailyBonusButton;

        private void Awake()
        {
            _letsPlayButton.onClick.AddListener(HandleLetsPlayButtonClick);
            _collectDailyBonusButton.onClick.AddListener(HandleCollectDailyBonusButtonClick);
        }

        private void OnDestroy()
        {
            _letsPlayButton.onClick.RemoveListener(HandleLetsPlayButtonClick);
            _collectDailyBonusButton.onClick.RemoveListener(HandleCollectDailyBonusButtonClick);
        }

        private void HandleLetsPlayButtonClick()
        {
            _letsPlayScreen.Close();
            _gameScreen.Open();
        }

        private void HandleCollectDailyBonusButtonClick()
        {
            _dailyBonusScreen.Close();
            _gameScreen.Open();
        }
    }
}