using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [RequireComponent(typeof(Image))]
    public class ChickenView : MonoBehaviour
    {
        [SerializeField] private Sprite _deadSprite;
        [SerializeField] private bool _autoDetectCollision = true;

        private Collider2D _collider;
        private Image _image;
        private Sprite _chickenSprite;
        private bool _isDead;

        public event Action OnChickenDie;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _image = GetComponent<Image>();
            _chickenSprite = _image != null ? _image.sprite : null;

            if (_collider == null)
                Debug.LogWarning($"{nameof(ChickenView)}: Collider2D is missing on the chicken GameObject.", this);
        }

        private void Update()
        {
            if (!_autoDetectCollision || _isDead)
                return;

            if (_collider == null)
                return;

            CheckPhysicsCollision();
        }

        private void CheckPhysicsCollision()
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(_collider.bounds.center);

            foreach (var collider in colliders)
            {
                if (collider == null || collider.gameObject == gameObject)
                    continue;

                if (collider.TryGetComponent<UI.Views.CarView>(out var car))
                {
                    HandleHitByCar();
                    return;
                }
            }
        }

        private void HandleHitByCar()
        {
            if (_isDead)
                return;

            _isDead = true;

            if (_deadSprite != null && _image != null)
                _image.sprite = _deadSprite;

            OnChickenDie?.Invoke();
        }

        public void ChangeSprite(Sprite sprite)
        {
            if (sprite == null || _image == null)
                return;

            _chickenSprite = sprite;
            _image.sprite = sprite;
        }
    }
}