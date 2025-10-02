using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TD.HomeScreen.BottomBar
{
    public class BottomBarView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject indicator;
        [SerializeField] private BottomBarButton startSelected;
        [SerializeField] private bool includeDisabledButtons;
        
        private List<BottomBarButton> _footerButtons = new List<BottomBarButton>();
        private BottomBarButton _buttonSelected;
        private GameObject _currentSlot;

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
            GetComponentsInChildren(includeInactive: includeDisabledButtons, _footerButtons);
            
            foreach (var btn in _footerButtons)
            {
                btn.OnButtonClickedEvent.AddListener(OnButtonClickedEvent);
            }
        }

        private void OnDisable()
        {
            foreach (var btn in _footerButtons)
            {
                btn.OnButtonClickedEvent.RemoveListener(OnButtonClickedEvent);
            }
        }
        
        private void OnButtonClickedEvent(BottomBarButton buttonClicked)
        {
            if (_footerButtons.Contains(buttonClicked))
            {
                if (_buttonSelected == buttonClicked)
                {
                    _buttonSelected = null;
                    _currentSlot = null;

                    foreach (var btn in _footerButtons)
                    {
                        btn.SetSelect(false);
                    }

                    indicator.SetActive(false);

                    return;
                }

                _buttonSelected = buttonClicked;

                foreach (var btn in _footerButtons)
                {
                    btn.SetSelect(_buttonSelected == btn);
                }

                MoveIndicator();
            }
        }

        private void MoveIndicator()
        {
            if (_buttonSelected == null) return;
            if (_currentSlot == _buttonSelected.gameObject) return;

            _currentSlot = _buttonSelected.gameObject;

            indicator.SetActive(true);
            indicator.transform.DOKill();
            indicator.transform.DOMoveX(_currentSlot.transform.position.x, .25f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                indicator.transform.position = new Vector3(_currentSlot.transform.position.x,
                                                            indicator.transform.position.y,
                                                            indicator.transform.position.z);
            });
        }
    }
}
