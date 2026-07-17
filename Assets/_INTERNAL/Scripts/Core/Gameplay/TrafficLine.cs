using DG.Tweening;
using System.Collections.Generic;
using UI.Views;
using UnityEngine;

namespace Core.Gameplay
{
    public class TrafficLine : MonoBehaviour
    {
        [Header("Points (RectTransform, same canvas)")]
        [SerializeField] private RectTransform _spawnPoint;
        [SerializeField] private RectTransform _targetPoint;

        [Header("Car prefab and visuals")]
        [SerializeField] private CarView _carPrefab;
        [SerializeField] private List<Sprite> _carSprites = new();

        [Header("Line settings")]
        [SerializeField] private int _laneIndex = 0;

        [field: Header("Spawn/Speed ranges")]
        [field: SerializeField] public float MinSpawnDelay { get; private set; } = 1.0f;
        [field: SerializeField] public float MaxSpawnDelay { get; private set; } = 3.0f;
        [field: SerializeField] public float MinCarSpeed { get; private set; } = 0.8f;
        [field: SerializeField] public float MaxCarSpeed { get; private set; } = 2.0f;

        private CarView _carView;

        private bool _isOccupied = false;
        private bool _isActive = true;

        public bool IsOccupied => _isOccupied;
        public bool IsActive => _isActive;
        public int LaneIndex => _laneIndex;
        public bool IsValid => _spawnPoint != null && _targetPoint != null && _carPrefab != null;

        public void DestroyCar(float delay = 0f)
        {
            if(_carView != null)
            {
                Destroy(_carView.gameObject, delay);
                _carView = null;
            }
        }

        public void DeactiveLine() => _isActive = false;

        public void SpawnCar()
        {
            if (!IsValid)
            {
                Debug.LogWarning($"{nameof(TrafficLine)}: Line is not configured correctly.", this);
                return;
            }

            if (_isOccupied || !_isActive) 
                return;

            _isOccupied = true;
           _carView = Object.Instantiate(_carPrefab, _spawnPoint.parent, false);

            _carView.transform.position = _spawnPoint.position;

            if (_carView != null && _carSprites != null && _carSprites.Count > 0)
            {
                var sprite = _carSprites[Random.Range(0, _carSprites.Count)];
                _carView.SetCarSprite(sprite);
            }

            float speed = Random.Range(MinCarSpeed, MaxCarSpeed);

            if (_carView != null)
            {
                _carView.OnMovementCompete += () => _isOccupied = false;
                _carView.PlayMove(_targetPoint.anchoredPosition, speed);
            }
        }
    }
}