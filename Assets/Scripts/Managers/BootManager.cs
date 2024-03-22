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
        yield return SceneManager.LoadSceneAsync("_");
        foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (go.transform.parent == null) // This means it's a root object
            {
                Destroy(go);
            }
        }

        yield return new WaitForSeconds(3);
        yield return SceneManager.LoadSceneAsync("BootScene");
    }
}
