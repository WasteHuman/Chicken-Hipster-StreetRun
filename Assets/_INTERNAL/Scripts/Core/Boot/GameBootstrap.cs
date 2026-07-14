using System.Collections;
using UI.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Boot
{
    public class GameBootstrap
    {
        private static GameBootstrap _instance;

        private Coroutine _loadingCoroutine;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoStart()
        {
            _instance = new();

            Run();
        }
        
        private static void Run()
        {
            _instance.LoadMainScene();
        }

        private void LoadMainScene()
        {
            var loadingScreenViewPrefab = Resources.Load<UILoadingView>("Prefabs/UI/UILoadingView");
            var loadingScreenView = Object.Instantiate(loadingScreenViewPrefab);

            if (loadingScreenViewPrefab == null)
            {
                Debug.LogError($"[Game Bootstrap] Loading Screen View is null!");
                return;
            }

            var monoBehaviourHelper = new GameObject("[MONOBEHAVIOUR_HELPER]").AddComponent<MonoBehaviourHelper>();

            if(_loadingCoroutine != null)
                monoBehaviourHelper.StopCoroutine(_loadingCoroutine);

            _loadingCoroutine = monoBehaviourHelper.StartCoroutine(LoadMainSceneCoroutine(loadingScreenView));
        }

        private IEnumerator LoadMainSceneCoroutine(UILoadingView loadingScreenView)
        {
            loadingScreenView.ResetProgress();

            AsyncOperation asyncOp = SceneManager.LoadSceneAsync("Main");

            while (!asyncOp.isDone)
            {
                float progress = Mathf.Clamp01(asyncOp.progress / 0.9f);
                loadingScreenView.SetLoadingProgress(progress);
                yield return null;
            }

            _loadingCoroutine = null;

            loadingScreenView.ResetProgress();
        }
    }

    internal class MonoBehaviourHelper : MonoBehaviour { }
}