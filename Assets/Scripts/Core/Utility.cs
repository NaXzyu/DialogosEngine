using System.Threading.Tasks;

public static class Utility
{
    public static async void Quit()
    {
        await Task.Delay(5000); // Wait for 5 seconds

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
