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
        [Tooltip("If true, the button will start in a locked state and be non-interactable.")]
        [SerializeField] private bool startLocked;

        [Header("Events")] 
        [Tooltip("Event fired when the button is clicked.")]
        public UnityEvent<BottomBarButton> ButtonClicked;

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

        private void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(InvokeClickEvent);
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(InvokeClickEvent);
        }

        private void Start()
        {
            locked = startLocked;
        }
        
        private void InvokeClickEvent()
        {
            ButtonClicked?.Invoke(this);
        }
    }
}
