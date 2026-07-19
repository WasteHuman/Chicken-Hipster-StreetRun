using Scripts.Core;
using System;
using UI.Views;

namespace Core.Gameplay
{
    public class DifficultController : IDisposable
    {
        private readonly DifficultyDropdownView _dropDownView;

        private Difficulty _difficulty = Difficulty.Easy;

        public event Action<Difficulty> OnDifficultChaged;

        public DifficultController(DifficultyDropdownView view)
        {
            _dropDownView = view;

            _dropDownView.OnDifficultySelected += SetDifficult;
        }

        public void Dispose() => _dropDownView.OnDifficultySelected -= SetDifficult;

        private void SetDifficult(Difficulty difficulty)
        {
            if (_difficulty == difficulty)
                return;

            _difficulty = difficulty;

            OnDifficultChaged?.Invoke(difficulty);
        }
    }
}