using GetyourCrown.UI.UI_Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GetyourCrown.UI
{
    public class UI_Manager : UI_Singleton<UI_Manager>
    {
        public UI_Manager()
        {
            _uis = new Dictionary<Type, UI_Base>(EXPECTED_MAX_UI_COUNT_IN_SCENE);
            _popupStack = new List<UI_Popup>(EXPECTED_MAX_POPUP_COUNT_IN_SCENE);
        }

        public IEnumerable<UI_Popup> popups => _popupStack;

        const int EXPECTED_MAX_UI_COUNT_IN_SCENE = 20;
        const int EXPECTED_MAX_POPUP_COUNT_IN_SCENE = 5;
        Dictionary<Type, UI_Base> _uis;
        UI_Screen _screen;
        List<UI_Popup> _popupStack;

        public void Register(UI_Base ui)
        {
            if (_uis.TryAdd(ui.GetType(), ui))
            {
                //Debug.Log($"Registered UI {ui.GetType()}");
                if (ui is UI_Popup)
                {
                    ui.onShow += () => Push((UI_Popup)ui);
                    ui.onHide += () => Pop((UI_Popup)ui);
                }
            }
            else
            {
                throw new Exception($"Failed to register ui {ui.GetType()}. already exitst.");
            }
        }

        public void UnRegister(UI_Base ui)
        {
            if(_uis.Remove(ui.GetType()))
            {
                if(ui is UI_Popup)
                {
                    _popupStack.Remove((UI_Popup)ui);
                }
            }
        }

        public T Resolve<T>()
            where T : UI_Base
        {
            if (_uis.TryGetValue(typeof(T), out UI_Base result))
            {
                return (T)result;
            }
            else
            {
                string path = $"UI/Canvas - {typeof(T).Name.Substring(3)}";
                UI_Base prefab = Resources.Load<UI_Base>(path);

                if (prefab == null)
                    throw new Exception($"Failed to resolve ui {typeof(T)}. Not exist");

                return (T)GameObject.Instantiate(prefab);
            }
        }
        
        public void SetScreen(UI_Screen screen)
        {
            if (_screen != null)
            {
                _screen.inputActionsEnabled = false;
                _screen.Hide();
            }

            _screen = screen;
            _screen.sortingOrder = 0;
            _screen.inputActionsEnabled = true;
        }

        public void Push(UI_Popup popup)
        {
            
            int popupIndex = _popupStack.FindLastIndex(ui => ui == popup);

            if (popupIndex >= 0)
            {
                _popupStack.RemoveAt(popupIndex);
            }

            int sortingOrder = 1;

            if (_popupStack.Count > 0)
            {
                UI_Popup prevPopup = _popupStack[^1];
                prevPopup.inputActionsEnabled = false;
                sortingOrder = prevPopup.sortingOrder + 1;
            }

            popup.sortingOrder = sortingOrder;
            popup.inputActionsEnabled = true;
            _popupStack.Add(popup);
            //Debug.Log($"Push popup : {popup.name}");
        }

        public void Pop(UI_Popup popup)
        {
            int popupIndex = _popupStack.FindLastIndex(ui => ui == popup);

            if (popupIndex < 0)
            {
                //throw new Exception($"Failed to remove popup. {popup.name}");
                Debug.Log($"Failed to remove popup. {popup.name}");
                return;
            }

            if (popupIndex == _popupStack.Count -1)
            {
                _popupStack[popupIndex].inputActionsEnabled = false;

                if (popupIndex > 0)
                    _popupStack[popupIndex - 1].inputActionsEnabled = true;
            }

            _popupStack.RemoveAt(popupIndex);
            //Debug.Log($"Pop popup : {popup.name}");
        }
    }
}