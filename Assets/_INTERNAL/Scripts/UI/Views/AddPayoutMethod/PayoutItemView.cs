using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.AddPayoutMethod
{
    [RequireComponent(typeof(Button))]
    public class PayoutItemView : MonoBehaviour
    {
        [Header("Payout Element Setup")]
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _nameLabel;
        [SerializeField] private string _name;
        [SerializeField] private bool _isSecondInputNeeded = false;
        [SerializeField] private Button _connectButton;
        [SerializeField] private Image _connectButtonIcon;
        [SerializeField] private Sprite _connectedSprite;

        private TextMeshProUGUI _connectLabel;

        private bool _isConnected = false;

        public Sprite Icon => _icon.sprite;
        public string Name => _name;
        public bool IsSecondInputNeeded => _isSecondInputNeeded;

        public event Action<PayoutItemView> OnConnectButtonClicked;

        public void Initialize()
        {
            _connectButtonIcon = _connectButton.gameObject.GetComponent<Image>();
            _connectLabel = _connectButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();

            if (PlayerPrefs.HasKey($"{_name}_1"))
            {
                _isConnected = true;
                _connectButtonIcon.sprite = _connectedSprite;
                _connectLabel.text = "Connected!";
            }

            _connectButton.onClick.AddListener(HandleConnectButtonClick);

            _nameLabel.text = _name;
        }

        public void Dispose()
        {
            _connectButton.onClick.RemoveListener(HandleConnectButtonClick);
        }

        public void Connect(string firstResult, string secondResult = null)
        {
            _isConnected = true;
            _connectButtonIcon.sprite = _connectedSprite;
            _connectLabel.text = "Connected!";

            PlayerPrefs.SetInt($"{_name}_1", 1);

            if (!string.IsNullOrEmpty(secondResult))
            {
                PlayerPrefs.SetString($"{_name}_1", firstResult);
                PlayerPrefs.SetString($"{_name}_2", secondResult);
                return;
            }

            PlayerPrefs.SetString($"{_name}_1", firstResult);
        }

        private void HandleConnectButtonClick() => OnConnectButtonClicked?.Invoke(this);
    }
}