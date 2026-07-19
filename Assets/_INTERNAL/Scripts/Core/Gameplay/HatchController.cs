using Scripts.Core;
using System;
using System.Collections.Generic;
using UI.Views;
using UnityEngine;

namespace Core.Gameplay
{
    public class HatchController : MonoBehaviour
    {
        [SerializeField] private BetController _betController;
        [SerializeField] private List<HatchView> _hatches = new();

        public event Action OnGameWon;

        private void Awake()
        {
            foreach (var h in _hatches)
                h.OnHatchActivated += OnHatchActivated;
        }

        private void OnDestroy()
        {
            foreach (var h in _hatches)
            {
                if (h != null)
                    h.OnHatchActivated -= OnHatchActivated;
            }
        }

        public void SetEasyMultipliers()
        {
            foreach (var h in _hatches)
                h.SetEasyMultiplier();
        }

        public void SetMediumMultipliers()
        {
            foreach (var h in _hatches)
                h.SetMediumMultiplier();
        }

        public void SetHardMultipliers()
        {
            foreach (var h in _hatches)
                h.SetHardMultipler();
        }

        private void OnHatchActivated(float multiplier, HatchView hatch)
        {
            if (_betController != null)
                _betController.ApplyMultiplier(multiplier);

            Debug.Log($"[Hatch Controller] Mult: {multiplier}");

            if (IsLastHatch(hatch))
                OnGameWon?.Invoke();
        }

        private bool IsLastHatch(HatchView hatch)
        {
            return hatch == _hatches[^1];
        }

        public void ResetAll()
        {
            foreach (var h in _hatches)
                h?.ResetHatch();
        }

        public void Activate(HatchView hatch)
        {
            if (hatch == null)
                return;

            hatch.Activate();
        }
    }
}