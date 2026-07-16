using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Gameplay
{
    public class TrafficController : MonoBehaviour
    {
        [SerializeField] private List<TrafficLine> _lines = new();
        [SerializeField] private MovementController _movementController;

        private readonly List<Coroutine> _routines = new();

        private void Awake()
        {
            if (_lines == null || _lines.Count == 0)
                Debug.LogWarning($"{nameof(TrafficController)}: no traffic lines configured.", this);
        }

        public void StartAll()
        {
            StopAll();

            foreach (var line in _lines)
            {
                if (line == null)
                    continue;

                var c = StartCoroutine(SpawnRoutine(line));
                _routines.Add(c);
            }
        }

        public void StopAll()
        {
            foreach (var c in _routines)
                StopCoroutine(c);

            foreach (var line in _lines)
                line.DestroyCar();

            _routines.Clear();
        }

        private IEnumerator SpawnRoutine(TrafficLine line)
        {
            yield return new WaitForSeconds(Random.Range(0f, 1f));

            while (true)
            {
                float delay = Random.Range(line.MinSpawnDelay, line.MaxSpawnDelay);
                yield return new WaitForSeconds(delay);

                if (ShouldSkipSpawnForLine(line))
                    continue;

                line.SpawnCar();
            }
        }

        private bool ShouldSkipSpawnForLine(TrafficLine line)
        {
            if (_movementController == null)
                return false;

            bool chickenStandingOnThisLine =
                !_movementController.IsMoving && _movementController.CurrentStepIndex == line.LaneIndex;
            return chickenStandingOnThisLine;
        }
    }
}