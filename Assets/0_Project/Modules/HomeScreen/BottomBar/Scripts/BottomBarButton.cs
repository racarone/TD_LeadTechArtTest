using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TD.HomeScreen.BottomBar
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Button))]
    public class BottomBarButton : MonoBehaviour
    {
        private static readonly int LockedID = Animator.StringToHash("Locked");
        private static readonly int SelectedID = Animator.StringToHash("Selected");

        [Header("General")] 
        [SerializeField] private bool lockOnAwake;

        [Header("Events")] 
        public UnityEvent<BottomBarButton> OnButtonClickedEvent;

        private Animator _animator;
        private Button _button;
        private bool _selected;
        private bool _locked;

        private void Awake()
        {
            SetLock(lockOnAwake);
        }

        private void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(() => { OnButtonClickedEvent?.Invoke(this); });
        }

        public void SetLock(bool locked)
        {
            _locked = locked;
            _button.interactable = !_locked;
            _animator.SetBool(LockedID, _locked);
        }

        public void SetSelect(bool selected)
        {
            _selected = selected;
            _animator.SetBool(SelectedID, _selected);
        }
    }
}
