using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.AddPayoutMethod
{
    public class ExchangePayoutItemView : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        public Sprite Icon => _icon.sprite;

        public void SetupIcon(Sprite sprite) => _icon.sprite = sprite;
    }
}