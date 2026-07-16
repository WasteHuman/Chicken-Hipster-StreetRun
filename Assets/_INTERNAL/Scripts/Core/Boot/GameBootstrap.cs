using Core.Services.Analytics;
using Io.AppMetrica;
using System.Collections;
using System.Threading.Tasks;
using UI.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Boot
{
    public class GameBootstrap
    {
        private static GameBootstrap _instance;

        private AnalyticsService _analyticsService;

        private Coroutine _loadingCoroutine;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async Task AutoStart()
        {
            _instance = new();

            await InitializeExternalSDK();
            Run();
        }
        
        private static async Task InitializeExternalSDK()
        {
            CheckFirstLaunch();

            var analyticsServicePrefab = Resources.Load<AnalyticsService>("Prefabs/Services/[ANALYTICS_SERVICE]");
            if(analyticsServicePrefab == null)
            {
                Debug.LogError($"[Game Bootstrap] Analytics Service prefab is null!");
                return;
            }

            _instance._analyticsService = Object.Instantiate(analyticsServicePrefab);

            try
            {
                AppMetrica.Activate(new AppMetricaConfig("4bb5f44f-8e5f-4847-8998-695879cf41f6")
                {
                    FirstActivationAsUpdate = !IsFirstLaunch()
                });
                Debug.Log($"[GlobalAction Bootstrap] AppMetrica initialized successfully");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[GlobalAction Bootstrap] Failed to initialize AppMetrica: {ex.Message}");
            }
        }

        private static void Run()
        {
            _instance.LoadMainScene();
        }

        private static bool IsFirstLaunch()
        {
            // TODO: Сделать проверку не только по ключу PlayerPrefs, но и по другим критериям
            if (!PlayerPrefs.HasKey("First_Launch"))
                return false;

            return true;
        }

        private static void CheckFirstLaunch()
        {
            if (PlayerPrefs.HasKey("First_Launch"))
                PlayerPrefs.SetInt("First_Launch", 1);
        }

        private void LoadMainScene()
        {
            var loadingScreenViewPrefab = Resources.Load<UILoadingView>("Prefabs/UI/UILoadingView");
            var loadingScreenView = Object.Instantiate(loadingScreenViewPrefab);

            if (loadingScreenViewPrefab == null)
            {
                Debug.LogError($"[GlobalAction Bootstrap] Loading Screen View is null!");
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
            _analyticsService.ReportGameStart();
        }
    }

    internal class MonoBehaviourHelper : MonoBehaviour { }
}