using UnityEngine;

namespace TD.HomeScreen.SettingsPopup
{
    public class SettingsPopupController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private static readonly int CloseId = Animator.StringToHash("Close");
        
        public void OnCloseButtonClicked()
        {
            animator.SetTrigger(CloseId);
        }
        
        // Method called through animation event when the close animation is completed
        public void OnClosedAnimationCompleted()
        {
            gameObject.SetActive(false);
        }
    }
}
