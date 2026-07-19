using Scripts.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [RequireComponent(typeof(Button))]
    public class DifficultyItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private DifficultyDropdownView _dropDown;
        [SerializeField] private Difficulty _difficulty;

        private Button _select;

        private void Awake()
        {
            _select = GetComponent<Button>();

            _label.text = _difficulty.ToString();

            _select.onClick.AddListener(Select);
        }

        private void OnDestroy()
        {
            _select.onClick.RemoveListener(Select);
        }

        private void Select() => _dropDown.Select(_difficulty);
    }
}