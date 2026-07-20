using System;
using UI.Views;
using UnityEngine;

namespace Core.Gameplay
{
    public class ChickenController : MonoBehaviour
    {
        [SerializeField] private ChickenView _view;
        [SerializeField] private Sprite _idleSprite;
        [SerializeField] private Sprite _moveSprite;
        [SerializeField] private Sprite _deathSprite;

        private bool _isChickenDie = false;

        public bool IsChickenDie => _isChickenDie;

        public event Action OnChickenDie;

        private void Awake()
        {
            _view.OnChickenDie += HandleChickenDie;
        }

        private void OnDestroy()
        {
            _view.OnChickenDie -= HandleChickenDie;
        }

        public void ChickenMove()
        {
            if (_isChickenDie)
                return;

            _view.ChangeSprite(_moveSprite);
        }

        public void ChickenDie()
        {
            _view.ChangeSprite(_deathSprite);
        }

        public void ChickenIdle()
        {
            if (_isChickenDie)
                return;

            _view.ChangeSprite(_idleSprite);
        }

        private void HandleChickenDie()
        {
            _isChickenDie = true;
            OnChickenDie?.Invoke();
        }
    }
}