using Core.Exchange;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Views.AddPayoutMethod
{
    public class AddPayoutMethodView : MonoBehaviour
    {
        [SerializeField] private ExchangeController _exchangeController;
        [SerializeField] private List<PayoutItemView> _payoutItems = new();
        [SerializeField] private PayoutItemWindowView _window;

        private PayoutItemView _openedItem;

        private void Awake()
        {
            foreach(var item in _payoutItems)
            {
                item.Initialize();
                item.OnConnectButtonClicked += HandleConnectButtonClick;
            }

            _window.Initialize();
            _window.OnSaveButtonClicked += HandleSaveButtonClick;
        }

        private void OnDestroy()
        {
            foreach(var item in _payoutItems)
            {
                item.Dispose();
                item.OnConnectButtonClicked -= HandleConnectButtonClick;
            }

            _window.Dispose();
            _window.OnSaveButtonClicked -= HandleSaveButtonClick;
        }

        private void HandleConnectButtonClick(PayoutItemView item)
        {
            _window.ResetInputFields();

            _openedItem = item;

            _window.SetupIcon(item.Icon);
            _window.SetupPayoutName(item.Name);
            _window.SetupInputFields(item.IsSecondInputNeeded);

            _window.Open();
        }

        private void HandleSaveButtonClick()
        {
            if (_openedItem == null)
                return;

            _openedItem.Connect(_window.FirstInputResult, _window.SecondInputResult);
            _exchangeController.AddPayoutMethod(_openedItem.Icon);
        }
    }
}