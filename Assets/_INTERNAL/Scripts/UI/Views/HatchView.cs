using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Views
{
    [RequireComponent(typeof(Image))]
    public class HatchView : MonoBehaviour
    {
        [SerializeField] private float _hatchMultiplier = 1f;
        [SerializeField] private Sprite _activatedSprite;
        [SerializeField] private bool _autoActivateOnCollision = true;

        private Collider2D _collider;
        private Image _image;
        private Sprite _originalSprite;
        private bool _activated;

        public float Multiplier => _hatchMultiplier;
        public bool IsActivated => _activated;

        public event Action<float, HatchView> OnHatchActivated;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _image = GetComponent<Image>();
            _originalSprite = _image != null ? _image.sprite : null;
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

        public void Activate()
        {
            if (_activated) 
                return;

            _activated = true;

            if (_activatedSprite != null && _image != null)
                _image.sprite = _activatedSprite;

            OnHatchActivated?.Invoke(_hatchMultiplier, this);
        }

        public void ResetHatch()
        {
            _activated = false;

            if (_image != null && _originalSprite != null)
                _image.sprite = _originalSprite;
        }
    }
}