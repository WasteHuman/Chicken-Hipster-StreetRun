using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Views
{
    [RequireComponent(typeof(Button))]
    public class BackToMainMenuButton : MonoBehaviour
    {
        [SerializeField] private string _nameTargetScene;

        private Button _backButton;

        private void Awake()
        {
            _backButton = GetComponent<Button>();

            _backButton.onClick.AddListener(HandleBackButtonClick);
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(HandleBackButtonClick);
        }

        private void HandleBackButtonClick() => SceneManager.LoadSceneAsync(_nameTargetScene);
    }
}