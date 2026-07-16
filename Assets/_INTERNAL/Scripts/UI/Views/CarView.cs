using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CarView : MonoBehaviour
    {
        private Image _image;
        private RectTransform _rect;
        private Sprite _carSprite;
        private Tween _moveTween;

        public event Action OnMovementCompete;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _rect = GetComponent<RectTransform>();
            _carSprite = _image.sprite;

            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;

            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = false;
        }

        private void OnDestroy()
        {
            _moveTween?.Kill();
        }

        public void SetCarSprite(Sprite sprite)
        {
            if (sprite == null)
                return;

            if (_carSprite == null || _image == null)
            {
                Destroy(gameObject);
                return;
            }

            _carSprite = sprite;
            _image.sprite = _carSprite;
        }

        public void PlayMove(Vector2 targetAnchoredPosition, float duration, Ease ease = Ease.Linear)
        {
            _moveTween?.Kill();
            _moveTween = _rect
                .DOAnchorPos(targetAnchoredPosition, duration)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    OnMovementCompete?.Invoke();

                    if (gameObject != null)
                        Destroy(gameObject);
                });
        }

        public void Stop()
        {
            _moveTween?.Kill();
        }
    }
}