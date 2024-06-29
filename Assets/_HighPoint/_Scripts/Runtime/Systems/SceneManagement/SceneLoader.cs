using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.SceneManagement
{
    public class SceneGroupIds
    {
        static public int MAIN_MENU = 0;
        static public int MAP = 1;
        static public int BATTLE = 2;
    }

    public class SceneLoader : Singleton<SceneLoader>
    {
        [SerializeField] Image loadingBar;
        [SerializeField] float fillSpeed = 0.5f;
        [SerializeField] CanvasGroup loadingCanvas;
        [SerializeField] Camera loadingCamera;
        [SerializeField] SceneGroup[] sceneGroups;

        float targetProgress;
        bool isLoading;

        public readonly SceneGroupManager manager = new SceneGroupManager();

        protected override void OnAwake()
        {
            // TODO can remove
            // manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            // manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            // manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");
        }

        async void Start()
        {
#if UNITY_EDITOR
            await LoadSceneGroup(SceneGroupIds.MAP);
            // TODO get this working
            // if (Bootstrapper.EditorScene == SceneIds.MAP_NAME)
            // {
            //     await LoadSceneGroup(SceneGroupIds.MAP);
            // }
            // else if (Bootstrapper.EditorScene == SceneIds.BATTLE_NAME)
            // {
            //     await LoadSceneGroup(SceneGroupIds.BATTLE);
            // } 
#else
        await LoadSceneGroup(SceneGroupIds.MAIN_MENU);  
#endif
        }

        void Update()
        {
            if (!isLoading) return;

            float currentFillAmount = loadingBar.fillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

            float dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        public async Task LoadSceneGroup(int index)
        {
            loadingBar.fillAmount = 0f;
            targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);

            EnableLoadingCanvas();
            await manager.LoadScenes(sceneGroups[index], progress);
            EnableLoadingCanvas(false);
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        const float ratio = 1f;

        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}