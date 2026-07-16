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

        private void Awake()
        {
            _uiController.OnGoButtonClicked += HandleGoButtonClick;
            _uiController.OnGameStarted += HandleStartedGame;
            _uiController.OnCashOutClicked += HandleCashOutButtonClick;
            _uiController.OnRewardClaimed += HandleClaimedReward;

            _movementController.OnStepCompleted += HandleCompletedStep;
            _chickenController.OnChickenDie += HandleChickenDie;

            _betController.OnBetChanged += HandleBetChanged;

            _hatchController.OnGameWon += HandleGameWon;

            _screensController.OnGamePrepared += HandlePreparedGame;
        }

        private void OnDestroy()
        {
            _uiController.OnGoButtonClicked -= HandleGoButtonClick;
            _uiController.OnGameStarted -= HandleStartedGame;
            _uiController.OnCashOutClicked -= HandleCashOutButtonClick;
            _uiController.OnRewardClaimed -= HandleClaimedReward;

            _movementController.OnStepCompleted -= HandleCompletedStep;
            _chickenController.OnChickenDie -= HandleChickenDie;

            _betController.OnBetChanged -= HandleBetChanged;

            _hatchController.OnGameWon -= HandleGameWon;

            _screensController.OnGamePrepared -= HandlePreparedGame;
        }

        private void RestartGame(float betAmount = 0f, bool isLose = false)
        {
            if (isLose)
                EconomyController.Instance.Spend(betAmount);
            else
                EconomyController.Instance.Add(betAmount);

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

        private void HandleBetChanged(float newBetAmount) => _uiController.UpdateCashOutText(newBetAmount);

        private void HandleCashOutButtonClick()
        {
            RestartGame(_betController.GetCurrentBet());
        }

        private void HandleGameWon()
        {
            _uiController.ShowVictory(_betController.GetCurrentBet());
            _trafficController.StopAll();
        }

        private void HandleClaimedReward()
        {
            RestartGame(_betController.GetCurrentBet());
        }

        private void HandleChickenDie()
        {
            RestartGame(_betController.GetCurrentBet(), true);
        }

        private IEnumerator ReloadGame()
        {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadSceneAsync(SceneNames.MAIN);
        }
    }
}