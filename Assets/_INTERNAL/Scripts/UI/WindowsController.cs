using UI.Other;
using UnityEngine;

namespace UI
{
    public class WindowsController : MonoBehaviour
    {
        [Header("Windows")]
        [SerializeField] private Window _gamePanelIdle;
        [SerializeField] private Window _gamePanelPlay;
        [SerializeField] private Window _settigsWindow;

        [Space(5), Header("Controllers")]
        [SerializeField] private ButtonsController _buttonsController;

        private void Awake()
        {
            _buttonsController.OnSettingsButtonClick += HandleSettingsButtonClick;
        }

        private void OnDestroy()
        {
            _buttonsController.OnSettingsButtonClick -= HandleSettingsButtonClick;
        }

        private void HandleSettingsButtonClick() => _settigsWindow.Open();
    }
}