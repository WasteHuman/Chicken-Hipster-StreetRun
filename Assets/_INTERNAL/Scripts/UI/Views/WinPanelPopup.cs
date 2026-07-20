using Core.Services.AdsService;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UI.Other;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class WinPanelPopup : Window
    {
        [SerializeField] private bool _isDebug = false;
        [SerializeField] private GameObject _wheelOfLuckView;
        [SerializeField] private Button _claimButton;
        [SerializeField] private Button _boostRewardButton;
        [SerializeField] private TextMeshProUGUI _winAmountText;
        [SerializeField] private float _toggleAnimationDuration = 0.5f;
        [SerializeField] private RectTransform _rectTransform;

        private Tween _openTween;
        private Tween _closeTween;

        private float _currentWinAmount;

        private Coroutine _boostRewardCoroutine;

        public event Action OnClaimClicked;
        public event Action OnBoostRewardClicked;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.localScale = Vector3.zero;

            _claimButton.onClick.AddListener(HandleClaimButtonClick);
            _boostRewardButton.onClick.AddListener(HandleBoostRewardButtonClick);
        }

        private void OnDestroy()
        {
            _openTween?.Kill();
            _closeTween?.Kill();

            _claimButton.onClick.RemoveListener(HandleClaimButtonClick);
            _boostRewardButton.onClick.RemoveListener(HandleBoostRewardButtonClick);
        }

        private void OnDisable()
        {
            _openTween?.Kill();
            _closeTween?.Kill();

            _claimButton.gameObject.SetActive(false);
        }

        public void SetWinAmount(float amount)
        {
            _currentWinAmount = amount;

            if (_winAmountText != null)
                _winAmountText.text = $"{amount:N0} USDT";
        }

        public override void Open()
        {
            _boostRewardButton.gameObject.transform.localScale = Vector3.zero;

            gameObject.SetActive(true);

            _openTween?.Kill();

            _openTween = _rectTransform
                .DOScale(Vector2.one, _toggleAnimationDuration)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    _boostRewardCoroutine = StartCoroutine(BoostRewardDisapbleDelay());
                });
        }

        public override void Close()
        {
            _closeTween = _rectTransform
                .DOScale(Vector2.zero, _toggleAnimationDuration * 0.5f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    if (_boostRewardCoroutine != null)
                        StopCoroutine(_boostRewardCoroutine);

                    base.Close();
                });
        }

        private void HandleClaimButtonClick()
        {
            _openTween?.Kill();
            OnClaimClicked?.Invoke();
        }

        private void HandleBoostRewardButtonClick()
        {
            if (AdsController.Instance.IsRewardedAdLoaded())
            {
                AdsController.Instance.ShowRewardedAd(() =>
                {
                    _openTween?.Kill();
                    _wheelOfLuckView.SetActive(true);
                    OnBoostRewardClicked?.Invoke();
                });
                return;
            }

            if (_isDebug)
            {
                _openTween?.Kill();
                _wheelOfLuckView.SetActive(true);
                OnBoostRewardClicked?.Invoke();
            }
        }

        private IEnumerator BoostRewardDisapbleDelay()
        {
            if (!_boostRewardButton.gameObject.activeSelf)
                _boostRewardButton.gameObject.SetActive(true);

            _boostRewardButton.gameObject.transform
                .DOScale(Vector3.one, 0.5f).SetEase(Ease.OutSine);

            yield return new WaitForSeconds(2.5f);

            _boostRewardButton.gameObject.transform
                .DOScale(Vector3.zero, 0.5f).SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    _boostRewardButton.gameObject.SetActive(false);
                    _claimButton.gameObject.SetActive(true);
                });
        }
    }
}