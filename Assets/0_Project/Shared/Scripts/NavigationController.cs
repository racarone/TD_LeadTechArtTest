using UnityEngine;
using UnityEngine.SceneManagement;

namespace TD.Shared
{
    public class NavigationController : MonoBehaviour
    {
        /// <summary>
        /// Loads a scene by its name. Can be called from UI Button OnClick event.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}