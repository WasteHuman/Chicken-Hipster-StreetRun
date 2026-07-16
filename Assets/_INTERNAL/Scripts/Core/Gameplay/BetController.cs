using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Gameplay
{
    public class BetController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _minButton;
        [SerializeField] private Button _maxButton;
        [SerializeField] private Button _inc1;
        [SerializeField] private Button _inc2;
        [SerializeField] private Button _inc5;
        [SerializeField] private Button _inc10;

        [Space(5), Header("Bet Input")]
        [SerializeField] private TMP_InputField _betInput;
        [SerializeField] private TextMeshProUGUI _currentBetText;

        private const int MinBet = 1;
        private float _currentBet = MinBet;

        public event Action<float> OnBetChanged;

        private void Awake()
        {
            if (_betInput != null)
                _betInput.onEndEdit.AddListener(OnInputEdit);

            _minButton?.onClick.AddListener(SetMin);
            _maxButton?.onClick.AddListener(SetMax);

            _inc1?.onClick.AddListener(() => ChangeBy(1));
            _inc2?.onClick.AddListener(() => ChangeBy(2));
            _inc5?.onClick.AddListener(() => ChangeBy(5));
            _inc10?.onClick.AddListener(() => ChangeBy(10));

            RefreshInput();
        }

        private void OnDestroy()
        {
            if (_betInput != null)
                _betInput.onEndEdit.RemoveListener(OnInputEdit);
        }

        private void OnInputEdit(string raw)
        {
            if (int.TryParse(raw, out var v))
                SetBet(v);
            else
                RefreshInput();
        }

        public void SetMin() => SetBet(MinBet);

        public void SetMax()
        {
            if (EconomyController.Instance != null)
            {
                int max = Mathf.Max(MinBet, Mathf.FloorToInt(EconomyController.Instance.GetBalance()));
                SetBet(max);
            }
            else
                SetBet(MinBet);
        }

        public void ChangeBy(int delta)
        {
            var current = Mathf.FloorToInt(_currentBet);
            if (!EconomyController.Instance.HasEnoughBalance(current))
                return;

            SetBet(current + delta);
        }

        public void SetBet(int value)
        {
            int max = MinBet;
            if (EconomyController.Instance != null)
                max = Mathf.Max(MinBet, Mathf.FloorToInt(EconomyController.Instance.GetBalance()));

            _currentBet = Mathf.Clamp(value, MinBet, max);

            _betInput.SetTextWithoutNotify(_currentBet.ToString("N0"));

            _currentBetText.text = _currentBet.ToString("N0");
            OnBetChanged?.Invoke(_currentBet);
        }

        private void RefreshInput()
        {
            if (_betInput != null)
                _betInput.SetTextWithoutNotify(Mathf.FloorToInt(_currentBet).ToString());
        }

        public float GetCurrentBet() => _currentBet;

        public void ApplyMultiplier(float multiplier)
        {
            if (multiplier <= 0f)
                return;

            _currentBet *= multiplier;
            _currentBetText.text = _currentBet.ToString("N0");

            OnBetChanged?.Invoke(_currentBet);
        }
    }
}