using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TD.HomeScreen.BottomBar
{
    public class BottomBarView : MonoBehaviour
    {
        [Header("Elements")]
        [Tooltip("Indicator that highlights the selected button.")]
        [SerializeField] private BottomBarIndicator indicator;
        [Tooltip("Button that is selected when the view is opened.")]
        [SerializeField] private BottomBarButton startSelected;
        [Tooltip("Include disabled buttons when searching for buttons in children.")]
        [SerializeField] private bool includeDisabledButtons;

        [Header("Events")] 
        [Tooltip("Event fired when a button is selected.")]
        public UnityEvent<BottomBarView> ContentActivated;
        [Tooltip("Event fired when the selected button is deselected.")]
        public UnityEvent<BottomBarView> Closed;
        
        private List<BottomBarButton> _buttons = new List<BottomBarButton>();
        private BottomBarButton _selectedButton;
        
        /// <summary>
        /// Currently selected button. Null if no button is selected.
        /// </summary>
        public BottomBarButton selectedButton => _selectedButton;
        
        /// <summary>
        /// Selects the given button. If the button is already selected, it will be deselected.
        /// If null is passed, the currently selected button will be deselected.
        /// </summary>
        public void SelectButton(BottomBarButton button)
        {
            if (_selectedButton == button || button == null)
            {
                if (_selectedButton)
                {
                    _selectedButton.selected = false;
                    _selectedButton = null;
                }
                
                indicator.ClearTarget();
                Closed?.Invoke(this);
            }
            else
            {
                if (_selectedButton)
                    _selectedButton.selected = false;

                _selectedButton = button;
                _selectedButton.selected = true;
                indicator.SetTarget((RectTransform)_selectedButton.transform);
                ContentActivated?.Invoke(this);
            }
        }

        private void Start()
        {
            if (startSelected != null)
            {
                OnButtonClickedEvent(startSelected);
            }
        }

        private void OnEnable()
        {
            GetComponentsInChildren(includeInactive: includeDisabledButtons, _buttons);
            
            foreach (var btn in _buttons)
            {
                btn.OnButtonClickedEvent.AddListener(OnButtonClickedEvent);
            }
        }

        private void OnDisable()
        {
            foreach (var btn in _buttons)
            {
                btn.OnButtonClickedEvent.RemoveListener(OnButtonClickedEvent);
            }
            
            indicator.HideImmediate();
        }
        
        private void OnButtonClickedEvent(BottomBarButton buttonClicked)
        {
            SelectButton(buttonClicked);
        }
    }
}
