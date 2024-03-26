using System.Diagnostics;
using System.Threading.Tasks;

namespace DialogosEngine
{
    public static class Utility
    {
        public static async void Quit(int waitTime)
        {
            await Task.Delay(waitTime);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}