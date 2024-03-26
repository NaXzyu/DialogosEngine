using System.Threading.Tasks;
using UnityEngine;

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