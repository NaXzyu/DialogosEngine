using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootManager : MonoBehaviour
{
    private static BootManager _instance;

    public static BootManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var bootManagerObj = new GameObject("_BootManager");
                _instance = bootManagerObj.AddComponent<BootManager>();
                DontDestroyOnLoad(bootManagerObj);
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void RebootGame()
    {
        StartCoroutine(RebootGameCoroutine());
    }

    private IEnumerator RebootGameCoroutine()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            SceneManager.UnloadSceneAsync(scene);
        }

        yield return new WaitUntil(() => SceneManager.sceneCount == 0);

        yield return new WaitForSeconds(3);

        // Load the BootScene again
        SceneManager.LoadScene(0);
    }

}
