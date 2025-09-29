using UnityEngine;
using UnityEngine.SceneManagement;

namespace TD.Shared
{
    public class NavigationController : MonoBehaviour
    {
        //  Load scene by name
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}