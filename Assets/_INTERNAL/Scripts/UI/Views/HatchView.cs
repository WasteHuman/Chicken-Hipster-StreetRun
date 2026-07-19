using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [RequireComponent(typeof(Image))]
    public class HatchView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _multiplierLabel;
        [SerializeField] private float _baseMultiplier = 1f;
        [SerializeField] private Sprite _activatedSprite;
        [SerializeField] private bool _autoActivateOnCollision = true;

        [Space(5), Header("Multipliers By Difficulty")]
        [SerializeField] private float _easyMultiplier;
        [SerializeField] private float _mediumMultiplier;
        [SerializeField] private float _hardMultiplier;

        private Collider2D _collider;
        private Image _image;
        private Sprite _originalSprite;
        private bool _activated;

        public float Multiplier
        {
            get => _baseMultiplier;
            private set
            {
                _baseMultiplier = value;
                _multiplierLabel.text = $"{value:F2}x";
            }
        }
        public bool IsActivated => _activated;

        public event Action<float, HatchView> OnHatchActivated;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _image = GetComponent<Image>();
            _originalSprite = _image != null ? _image.sprite : null;

            if (_multiplierLabel == null)
                _multiplierLabel = GetComponentInChildren<TextMeshProUGUI>();

            _multiplierLabel.text = $"{_baseMultiplier}x";

            _easyMultiplier = _baseMultiplier;
            _mediumMultiplier = _baseMultiplier * 1.1f;
            _hardMultiplier = _baseMultiplier * 1.5f;
        }

        private void Update()
        {
            if (!_autoActivateOnCollision || _activated)
                return;

            CheckPhysicsCollision();
        }

        private void CheckPhysicsCollision()
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(_collider.bounds.center);

            foreach (var collider in colliders)
            {
                if (collider.gameObject == gameObject)
                    continue;

                if (collider.TryGetComponent<ChickenView>(out var chicken))
                {
                    Activate();
                    return;
                }
            }
        }

        public void SetEasyMultiplier() => Multiplier = _easyMultiplier;

        public void SetMediumMultiplier() => Multiplier = _mediumMultiplier;

        public void SetHardMultipler() => Multiplier = _hardMultiplier;

        public void Activate()
        {
            if (_activated) 
                return;

            _activated = true;

            if (_activatedSprite != null && _image != null)
                _image.sprite = _activatedSprite;

            _multiplierLabel.gameObject.SetActive(false);
            OnHatchActivated?.Invoke(_baseMultiplier, this);
        }

        public void ResetHatch()
        {
            _activated = false;
            _multiplierLabel.gameObject.SetActive(true);

            if (_image != null && _originalSprite != null)
                _image.sprite = _originalSprite;
        }
    }
}