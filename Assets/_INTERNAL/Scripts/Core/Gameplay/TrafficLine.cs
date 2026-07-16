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

        private bool _isOccupied;

        public bool IsOccupied => _isOccupied;
        public int LaneIndex => _laneIndex;
        public bool IsValid => _spawnPoint != null && _targetPoint != null && _carPrefab != null;

        public void SpawnCar()
        {
            if (!IsValid)
            {
                Debug.LogWarning($"{nameof(TrafficLine)}: Line is not configured correctly.", this);
                return;
            }

            if (_isOccupied) 
                return;

            _isOccupied = true;
            CarView instance = Object.Instantiate(_carPrefab, _spawnPoint.parent, false);

            instance.transform.position = _spawnPoint.position;

            if (instance != null && _carSprites != null && _carSprites.Count > 0)
            {
                var sprite = _carSprites[Random.Range(0, _carSprites.Count)];
                instance.SetCarSprite(sprite);
            }

            float speed = Random.Range(MinCarSpeed, MaxCarSpeed);

            if (instance != null)
            {
                instance.OnMovementCompete += () => _isOccupied = false;
                instance.PlayMove(_targetPoint.anchoredPosition, speed);
            }
        }
    }
}