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
        
        public bool locked
        {
            get => _locked;
            set
            {
                if (value != _locked)
                {
                    _locked = value;
                    _selected = false;
                    _button.interactable = !_locked;
                    _animator.SetBool(LockedID, _locked);
                }
            }
        }

        public bool selected
        {
            get => _selected;
            set
            {
                if (_locked) return;
                if (value != _selected)
                {
                    _selected = value;
                    _animator.SetBool(SelectedID, _selected);
                }
            }
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _button = GetComponent<Button>();
            locked = lockOnAwake;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(InvokeClickEvent);
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(InvokeClickEvent);
        }
        
        private void InvokeClickEvent()
        {
            OnButtonClickedEvent?.Invoke(this);
        }
    }
}
