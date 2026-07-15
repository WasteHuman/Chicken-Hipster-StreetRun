using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class CarView : MonoBehaviour
    {
        private Sprite _carSprite;

        private void Awake()
        {
            _carSprite = GetComponent<Image>().sprite;
        }
    }
}