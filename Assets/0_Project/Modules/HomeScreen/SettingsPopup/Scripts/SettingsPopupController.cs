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
        
        /// <summary>
        /// Called when the Popup_Closing state animation is completed.
        /// </summary>
        public void OnClosedAnimationCompleted()
        {
            gameObject.SetActive(false);
        }
    }
}
