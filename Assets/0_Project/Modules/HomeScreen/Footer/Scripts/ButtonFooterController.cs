using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TD.HomeScreen.Footer
{
    public class ButtonFooterController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private Button footerBtn;
        [SerializeField] private bool lockOnAwake;

        [Header("Events")]
        public UnityEvent<ButtonFooterController> OnButtonClickedEvent;

        private bool _selected;
        private bool _locked;
        
        private static readonly int LockedId = Animator.StringToHash("Locked");
        private static readonly int SelectedId = Animator.StringToHash("Selected");

        private void Awake()
        {
            SetLock(lockOnAwake);
        }

        private void Start()
        {
            footerBtn.onClick.AddListener(() =>
            {
                OnButtonClickedEvent?.Invoke(this);
            });
        }

        public void SetLock(bool locked)
        {
            _locked = locked;
            footerBtn.interactable = _locked == false;
            animator.SetBool(LockedId, _locked);
        }

        public void SetSelect(bool selected)
        {
            _selected = selected;
            animator.SetBool(SelectedId, _selected);
        }
    }
}
