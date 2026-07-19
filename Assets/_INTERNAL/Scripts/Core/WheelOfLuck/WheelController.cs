using Core.Gameplay;
using Core.Services.AdsService;
using Core.Services.Analytics;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.WheelOfLuck
{
    public class WheelController : MonoBehaviour
    {
        [Header("Wheel")]
        [SerializeField] private RectTransform _wheelTransform;
        [SerializeField] private RectTransform _pointer;

        [Space(5), Header("Views")]
        [SerializeField] private List<RewardView> _rewardViews = new();

        [Space(5), Header("Spin settings")]
        [SerializeField] private float _spinDuration = 4f;
        [SerializeField] private int _minFullRotations = 4;

        [Space(5), Header("Economy")]
        [SerializeField] private int _initialFreeSpins = 1;

        [Space(5), Header("Text")]
        [SerializeField] private TextMeshProUGUI _cooldownText;
        [SerializeField] private TextMeshProUGUI _spinsCountText;

        [Space(5), Header("Buttons")]
        [SerializeField] private Button _startSpinButton;
        [SerializeField] private Button _watchAndCollectButton;

        [Space(5), Header("Other")]
        [SerializeField] private bool _isMultipliersWheel = false;

        [Space(5), Header("Debug")]
        [SerializeField] private bool _isDebug = false;

        private const string PREF_FREE_SPINS = "Wheel_FreeSpins";
        private const string PREF_NEXT_AVAILABLE_TICKS = "Wheel_NextAvailableTicks";
        private static readonly TimeSpan COOLDOWN = TimeSpan.FromHours(12);

        private int _freeSpins;
        private DateTime _nextAvailableUtc;
        private bool _isSpinning;
        private WheelReward _pendingReward;
        private int _pendingIndex;

        private Coroutine _cooldownCoroutine;

        private Tween _spinTween;
        private Tween _pulseTween;

        public event Action<WheelReward> OnSpinStarted;
        public event Action<WheelReward> OnSpinFinished;
        public event Action OnStateChanged;
        public event Action<float> OnMultiplierDropped;

        private void Awake()
        {
            if (!_isMultipliersWheel)
            {
                AnalyticsService.Instance.ReportGameStart(SceneNames.WHEEL_OF_LUCK);

                LoadState();
                MapRewardsToViews();
                UpdateCooldownLabel();
                StartCooldownUpdater();
                UpdatePulseState();
                return;
            }
            else
                MapRewardsToViews();
        }

        private void Start()
        {
            if (_startSpinButton == null || _watchAndCollectButton == null)
                return;

            _startSpinButton.onClick.AddListener(PrepareAndStartSpin);
            _watchAndCollectButton.onClick.AddListener(ClaimWithAd);

            _watchAndCollectButton.gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            if (_initialFreeSpins < 0) 
                _initialFreeSpins = 0;
        }

        private void OnDisable()
        {
            _spinTween?.Kill();
            _pulseTween?.Kill();
        }

        private void OnDestroy()
        {
            _spinTween?.Kill();
            _pulseTween?.Kill();

            if (_startSpinButton == null || _watchAndCollectButton == null)
                return;

            _startSpinButton.onClick.RemoveListener(PrepareAndStartSpin);
            _watchAndCollectButton.onClick.RemoveListener(ClaimWithAd);

            StopCooldownUpdater();
        }

        [ContextMenu("DEBUG: Reset Cooldown")]
        private void ResetCooldownForDebug()
        {
            _nextAvailableUtc = DateTime.MinValue;
            _freeSpins = _initialFreeSpins;
            SaveState();
            UpdateCooldownLabel();
            UpdatePulseState();
            OnStateChanged?.Invoke();
            Debug.Log("[Wheel] DEBUG: cooldown and free spins reset");
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!_isDebug) 
                return;

            // Нажмите R в режиме Play для сброса кулдауна
            if (Input.GetKeyDown(KeyCode.R))
                ResetCooldownForDebug();
        }
#endif

        private void UpdatePulseState()
        {
            if (_wheelTransform == null)
                return;

            bool shouldPulse = CanSpin();

            if (shouldPulse)
            {
                if (_pulseTween == null || !_pulseTween.IsActive())
                {
                    _pulseTween?.Kill();
                    _pulseTween = _wheelTransform
                        .DOScale(1.05f, 1f)
                        .SetEase(Ease.OutSine)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetTarget(_wheelTransform);
                }
            }
            else
            {
                _pulseTween?.Kill();
                _pulseTween = null;
            }
        }

        private void LoadState()
        {
            _freeSpins = PlayerPrefs.GetInt(PREF_FREE_SPINS, _initialFreeSpins);
            long ticks = Convert.ToInt64(PlayerPrefs.GetString(PREF_NEXT_AVAILABLE_TICKS, "0"));
            _nextAvailableUtc = ticks == 0 ? DateTime.MinValue : new DateTime(ticks, DateTimeKind.Utc);

            _spinsCountText.text = $"Free Spins: {_freeSpins}";
        }

        private void SaveState()
        {
            PlayerPrefs.SetInt(PREF_FREE_SPINS, _freeSpins);
            PlayerPrefs.SetString(PREF_NEXT_AVAILABLE_TICKS, _nextAvailableUtc == DateTime.MinValue ? "0" : _nextAvailableUtc.Ticks.ToString());
            PlayerPrefs.Save();
        }

        private void MapRewardsToViews()
        {
            if (_rewardViews == null || _rewardViews.Count == 0)
                return;

            for (int i = 0; i < _rewardViews.Count; i++)
            {
                _rewardViews[i].Configure(i, _rewardViews.Count);
            }
        }

        public bool IsAvailable()
        {
            return DateTime.UtcNow >= _nextAvailableUtc;
        }

        public int GetFreeSpins() => _freeSpins;

        public bool CanSpin()
        {
            return !_isSpinning && _freeSpins > 0 && IsAvailable() && _rewardViews != null && _rewardViews.Count > 0;
        }

        public void PrepareAndStartSpin()
        {
            if (!CanSpin())
            {
                Debug.LogWarning("[Wheel] Невозможно начать спин. Проверьте CanSpin()");
                return;
            }

            _pulseTween?.Kill();

            _pendingIndex = SelectRewardIndexByWeight();
            _pendingReward = _rewardViews[_pendingIndex].Reward;

            OnSpinStarted?.Invoke(_pendingReward);

            StartTweenSpin(_pendingIndex);

            if (_startSpinButton == null)
                return;

            _startSpinButton.interactable = false;
        }

        private int SelectRewardIndexByWeight()
        {
            float total = 0f;
            foreach (var r in _rewardViews) total += Mathf.Max(0f, r.Reward.Weight);

            if (total <= 0f)
                return UnityEngine.Random.Range(0, _rewardViews.Count);

            float t = UnityEngine.Random.value * total;
            float accum = 0f;
            for (int i = 0; i < _rewardViews.Count; i++)
            {
                accum += Mathf.Max(0f, _rewardViews[i].Reward.Weight);
                if (t <= accum)
                    return i;
            }

            return _rewardViews.Count - 1;
        }

        private void StartTweenSpin(int targetIndex)
        {
            if (_wheelTransform == null)
            {
                Debug.LogWarning("[Wheel] Wheel Transform не назначен");
                return;
            }

            _isSpinning = true;
            _spinTween?.Kill();

            RewardView targetRewardView = _rewardViews[targetIndex];
            
            if (!targetRewardView.TryGetComponent<RectTransform>(out var rewardTransform))
            {
                Debug.LogWarning("[Wheel] RewardView dont have RectTransform");
                return;
            }

            Debug.Log($"[Wheel] Spinning for reward index {targetIndex}, reward: {_pendingReward}");
            Debug.Log($"[Wheel] RewardView reward: {targetRewardView.Reward}");
            Debug.Log($"[Wheel] Are they the same? {_pendingReward == targetRewardView.Reward}");

            float neededAngle = CalculateAngleToPerfectAlignment(rewardTransform);

            Debug.Log($"[Wheel] Pointer position: {_pointer.position}, Reward position: {rewardTransform.position}");
            Debug.Log($"[Wheel] Needed angle to align: {neededAngle}");

            float startAngle = NormalizeAngle(_wheelTransform.eulerAngles.z);
            float neededDelta = NormalizeAngle(neededAngle - startAngle);

            float totalDelta = _minFullRotations * 360f + neededDelta;
            float endAngle = _wheelTransform.eulerAngles.z + totalDelta;

            _spinTween = _wheelTransform.DORotate(new Vector3(0f, 0f, endAngle), _spinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    _wheelTransform.eulerAngles = new Vector3(0f, 0f, endAngle);

                    _freeSpins = Mathf.Max(0, _freeSpins - 1);
                    _nextAvailableUtc = DateTime.UtcNow.Add(COOLDOWN);
                    SaveState();

                    _isSpinning = false;

                    Debug.Log($"[Wheel] Reward: {_pendingReward}");

                    OnSpinFinished?.Invoke(_pendingReward);
                    OnStateChanged?.Invoke();

                    UpdateCooldownLabel();

                    if (_startSpinButton == null)
                        return;

                    UpdatePulseState();

                    _watchAndCollectButton.gameObject.SetActive(true);
                    _startSpinButton.interactable = false;
                });
        }

        private float CalculateAngleToPerfectAlignment(RectTransform rewardTransform)
        {
            Vector3 centerPos = _wheelTransform.position;

            float angleToReward = Mathf.Atan2(
                rewardTransform.position.y - centerPos.y,
                rewardTransform.position.x - centerPos.x
            ) * Mathf.Rad2Deg;

            float angleToPointer = Mathf.Atan2(
                _pointer.position.y - centerPos.y,
                _pointer.position.x - centerPos.x
            ) * Mathf.Rad2Deg;

            float neededRotation = angleToPointer - angleToReward;

            Debug.Log($"[Wheel] Angle to reward: {angleToReward}, Angle to pointer: {angleToPointer}");
            Debug.Log($"[Wheel] Needed rotation: {neededRotation}");

            return NormalizeAngle(neededRotation);
        }

        private static float NormalizeAngle(float a)
        {
            a %= 360f;
            if (a < 0) a += 360f;
            return a;
        }

        public void ClaimWithoutAd()
        {
            if (_pendingReward == null)
            {
                Debug.LogWarning("[Wheel] Нет ожидаемой награды для выдачи");
                return;
            }

            ApplyReward(_pendingReward, bonusMultiplier: 1);
            _pendingReward = null;
            OnStateChanged?.Invoke();
            UpdateCooldownLabel();
            UpdatePulseState();
        }

        public void ClaimWithAd()
        {
            if (_pendingReward == null)
            {
                Debug.LogWarning("[Wheel] No pending reward for give");
                return;
            }

            if (AdsController.Instance == null)
            {
                Debug.LogWarning("[Wheel] AdsController not found, give without ad");
                ClaimWithoutAd();
                return;
            }

            if (!AdsController.Instance.IsRewardedAdLoaded())
            {
                Debug.LogWarning("[Wheel] Rewarded ad not availvable, give without ad");
                ClaimWithoutAd();
                return;
            }

            AdsController.Instance.ShowRewardedAd(() =>
            {
                ApplyReward(_pendingReward, bonusMultiplier: 2);
                _pendingReward = null;
                OnStateChanged?.Invoke();
                UpdateCooldownLabel();
                _watchAndCollectButton.gameObject.SetActive(false);
                UpdatePulseState();
            });
        }

        private void ApplyReward(WheelReward reward, int bonusMultiplier)
        {
            if (reward == null) 
                return;

            switch (reward.Type)
            {
                case WheelReward.RewardType.Coins:
                    int coins = (int)reward.Amount * Math.Max(1, bonusMultiplier);
                    EconomyController.Instance.AddCoins(coins);
                    Debug.Log($"[Wheel] Given coins: {coins}");

                    AnalyticsService.Instance.ReportGameWin(SceneNames.WHEEL_OF_LUCK);
                    break;

                case WheelReward.RewardType.FreeSpin:
                    int spins = (int)reward.Amount * Math.Max(1, bonusMultiplier);
                    _freeSpins += spins;
                    SaveState();
                    Debug.Log($"[Wheel] Given free spins: {spins}");

                    AnalyticsService.Instance.ReportGameWin(SceneNames.WHEEL_OF_LUCK);
                    break;

                case WheelReward.RewardType.Nothing:
                    Debug.Log("[Wheel] Nothing to give");

                    AnalyticsService.Instance.ReportGameLoss(SceneNames.WHEEL_OF_LUCK);
                    break;
                case WheelReward.RewardType.Multiplier:
                    if (!_isMultipliersWheel)
                        return;
                    OnMultiplierDropped?.Invoke(reward.Amount);
                    break;
            }

            _watchAndCollectButton.gameObject.SetActive(false);
            _startSpinButton.interactable = true;
            UpdatePulseState();
        }

        public TimeSpan GetRemainingCooldown()
        {
            if (IsAvailable()) 
                return TimeSpan.Zero;

            return _nextAvailableUtc - DateTime.UtcNow;
        }

        private void StartCooldownUpdater()
        {
            if (_cooldownText == null) 
                return;
            if (_cooldownCoroutine != null) 
                StopCoroutine(_cooldownCoroutine);
            _cooldownCoroutine = StartCoroutine(CooldownRoutine());
        }

        private void StopCooldownUpdater()
        {
            if (_cooldownCoroutine != null)
            {
                StopCoroutine(_cooldownCoroutine);
                _cooldownCoroutine = null;
            }
        }

        private IEnumerator CooldownRoutine()
        {
            while (true)
            {
                UpdateCooldownLabel();
                yield return new WaitForSeconds(1f);
            }
        }

        private void UpdateCooldownLabel()
        {
            if (_cooldownText == null) 
                return;

            var remaining = GetRemainingCooldown();
            _cooldownText.text = FormatTimeSpan(remaining);
        }

        private static string FormatTimeSpan(TimeSpan ts)
        {
            if (ts <= TimeSpan.Zero)
                return "00:00:00";

            int hours = (int)ts.TotalHours;
            return $"{hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }
    }
}