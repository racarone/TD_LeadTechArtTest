using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace TD.HomeScreen.BottomBar
{
    public class BottomBarView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject indicator;
        [SerializeField] private BottomBarButton startSelected;
        [SerializeField] private bool includeDisabledButtons;

        [Header("Events")] 
        public UnityEvent<BottomBarView> ContentActivated;
        public UnityEvent<BottomBarView> Closed;
        
        private List<BottomBarButton> _buttons = new List<BottomBarButton>();
        private BottomBarButton _selectedButton;
        
        public BottomBarButton selectedButton => _selectedButton;
        
        public void SelectButton(BottomBarButton button)
        {
            if (_selectedButton == button || button == null)
            {
                if (_selectedButton)
                {
                    _selectedButton.selected = false;
                    _selectedButton = null;
                }
                
                indicator.transform.DOKill();
                indicator.SetActive(false);
                
                Closed?.Invoke(this);
            }
            else
            {
                if (_selectedButton)
                    _selectedButton.selected = false;

                _selectedButton = button;
                _selectedButton.selected = true;
                MoveIndicator(_selectedButton.transform);
                
                ContentActivated?.Invoke(this);
            }
        }

        private void Start()
        {
            if (startSelected != null)
            {
                OnButtonClickedEvent(startSelected);
            }
            else
            {
                indicator.SetActive(false);
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
            
            indicator.transform.DOKill();
        }
        
        private void OnButtonClickedEvent(BottomBarButton buttonClicked)
        {
            SelectButton(buttonClicked);
        }

        private void MoveIndicator(Transform target)
        {
            indicator.SetActive(true);
            indicator.transform.DOKill();
            indicator.transform.DOMoveX(target.transform.position.x, .25f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                indicator.transform.position = new Vector3(
                    target.transform.position.x, 
                    indicator.transform.position.y,
                    indicator.transform.position.z);
            });
        }
    }
}
