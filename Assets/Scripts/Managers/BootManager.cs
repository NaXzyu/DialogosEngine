using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using CommandTerminal;

public class BootManager : MonoBehaviour
{
    private static BootManager _instance;
    public const string k_CoroutineManager = "CoroutineManager";
    public const string k_BootManager = "_BootManager";
    public const string k_BootSceneName = "BootScene";
    public const string k_TempScene = "_";

    private class CoroutineManager : MonoBehaviour
    {
        private static CoroutineManager _coroutineInstance;

        public static CoroutineManager Instance
        {
            get
            {
                if (_coroutineInstance == null)
                {
                    var _managerObj = new GameObject(k_CoroutineManager);
                    _coroutineInstance = _managerObj.AddComponent<CoroutineManager>();
                    DontDestroyOnLoad(_managerObj);
                }
                return _coroutineInstance;
            }
        }

        public void StartPostDestructCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }
    }

    public static BootManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var bootManagerObj = new GameObject(k_BootManager);
                _instance = bootManagerObj.AddComponent<BootManager>();
                DontDestroyOnLoad(bootManagerObj);
            }
            return _instance;
        }
    }

    public void SelfDestruct()
    {
        CoroutineManager.Instance.StartPostDestructCoroutine(PostDestructCoroutine());
        Destroy(gameObject);
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator LoadBootScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(k_BootSceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(k_BootSceneName));
    }

    private IEnumerator WaitForBootScene()
    {
        while (SceneManager.GetActiveScene().name != k_BootSceneName)
        {
            yield return null;
        }
    }

    private void UnloadTemporaryScene(Scene tempScene)
    {
        SceneManager.UnloadSceneAsync(tempScene);
    }

    private IEnumerator PostDestructCoroutine()
    {
        yield return LoadBootScene();
        yield return WaitForBootScene();
        UnloadTemporaryScene(SceneManager.GetSceneByName(k_TempScene));
    }
}
