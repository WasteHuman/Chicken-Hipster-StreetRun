using Core.Services.Analytics;
using Core.Services.Audio;
using Core.WheelOfLuck;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Gameplay
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private MovementController _movementController;
        [SerializeField] private TrafficController _trafficController;
        [SerializeField] private UIController _uiController;
        [SerializeField] private ChickenController _chickenController;
        [SerializeField] private BetController _betController;
        [SerializeField] private HatchController _hatchController;
        [SerializeField] private ScreensController _screensController;
        [SerializeField] private WheelController _wheelController;

        private void Awake()
        {
            _uiController.OnGoButtonClicked += HandleGoButtonClick;
            _uiController.OnGameStarted += HandleStartedGame;
            _uiController.OnCashOutClicked += HandleCashOutButtonClick;
            _uiController.OnRewardClaimed += HandleClaimedReward;
            _uiController.OnBoostRewardClicked += HandleBoostRewardClicked;

            _movementController.OnStepCompleted += HandleCompletedStep;
            _chickenController.OnChickenDie += HandleChickenDie;

            _betController.OnBetChanged += HandleBetChanged;

            _hatchController.OnGameWon += HandleGameWon;

            _screensController.OnGamePrepared += HandlePreparedGame;

            _wheelController.OnMultiplierDropped += HandleDroppedMultiplier;
        }

        private void Start()
        {
            if (!EconomyController.Instance.HasEnoughBalance(1f))
                _uiController.SetGoButtonInteractableState(false);

            _uiController.UpdateCashOutText(_betController.GetCurrentBet());

            AnalyticsService.Instance.ReportGameStart(SceneNames.MAIN);
        }

        private void OnDestroy()
        {
            _uiController.OnGoButtonClicked -= HandleGoButtonClick;
            _uiController.OnGameStarted -= HandleStartedGame;
            _uiController.OnCashOutClicked -= HandleCashOutButtonClick;
            _uiController.OnRewardClaimed -= HandleClaimedReward;
            _uiController.OnBoostRewardClicked -= HandleBoostRewardClicked;

            _movementController.OnStepCompleted -= HandleCompletedStep;
            _chickenController.OnChickenDie -= HandleChickenDie;

            _betController.OnBetChanged -= HandleBetChanged;

            _hatchController.OnGameWon -= HandleGameWon;

            _screensController.OnGamePrepared -= HandlePreparedGame;

            _wheelController.OnMultiplierDropped -= HandleDroppedMultiplier;
        }

        private void RestartGame(float betAmount = 0f, bool isLose = false)
        {
            if (isLose)
            {
                EconomyController.Instance.Spend(betAmount);
                _chickenController.ChickenDie();
            }
            else
                EconomyController.Instance.Add(betAmount);

            _trafficController.StopAll();
            StartCoroutine(ReloadGame());
        }

        private void HandlePreparedGame()
        {
            _trafficController.StartAll();
        }

        private void HandleCompletedStep()
        {
            _chickenController.ChickenIdle();

            if (_movementController.CanMoveNextStep)
                _uiController.SetGoButtonInteractableState(true);
            else
                _uiController.SetGoButtonInteractableState(false);
        }

        private void HandleStartedGame()
        {
            if (EconomyController.Instance.HasEnoughBalance(_betController.GetCurrentBet()))
                EconomyController.Instance.Spend(_betController.GetCurrentBet());
        }

        private void HandleGoButtonClick()
        {
            _uiController.SetGoButtonInteractableState(false);
            _chickenController.ChickenMove();
            _movementController.MoveStep();
        }

        private void HandleBetChanged(float newBetAmount)
        {
            AnalyticsService.Instance.ReportBetChange(SceneNames.MAIN);
            _uiController.UpdateCashOutText(newBetAmount);
        }

        private void HandleCashOutButtonClick()
        {
            _uiController.SetGoButtonInteractableState(false);
            RestartGame(_betController.GetCurrentBet());
        }

        private void HandleGameWon()
        {
            _uiController.SetGoButtonInteractableState(false);
            _uiController.ShowVictory(_betController.GetCurrentBet());
            _trafficController.StopAll();

            AnalyticsService.Instance.ReportGameWin(SceneNames.MAIN);

            // Играть вин
            // TODO: Сделать адекватнее, через статические константы или типы
            AudioController.Instance.PlaySfx(1);
        }

        private void HandleClaimedReward()
        {
            RestartGame(_betController.GetCurrentBet());
        }

        private void HandleBoostRewardClicked()
        {
            _wheelController.PrepareAndStartSpin();
        }

        private void HandleChickenDie()
        {
            AnalyticsService.Instance.ReportGameLoss(SceneNames.MAIN);

            // Играть луз
            // TODO: Сделать адекватнее, через статические константы или типы
            AudioController.Instance.PlaySfx(0);
            _uiController.SetGoButtonInteractableState(false);
            RestartGame(_betController.GetCurrentBet(), true);
        }

        private void HandleDroppedMultiplier(float mult)
        {
            var bet = _betController.GetCurrentBet();
            bet *= mult;
            EconomyController.Instance.Add(bet);

            StartCoroutine(ReloadGame());
        }

        private IEnumerator ReloadGame()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadSceneAsync(SceneNames.MAIN);
        }
    }
}