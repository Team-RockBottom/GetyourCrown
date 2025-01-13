using GetYourCrown.Singletons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GetYourCrown.UI
{
    public class UI_Manager : Singleton<UI_Manager>
    {
        public UI_Manager()
        {
            _uis = new Dictionary<Type, UI_Base>(EXPECTED_MAX_UI_COUNT_IN_SCENE);
        }


        const int EXPECTED_MAX_UI_COUNT_IN_SCENE = 30;
        Dictionary<Type, UI_Base> _uis;
        UI_Screan _screan;


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
                    throw new Exception($"Failed to resolve ui {typeof(T)}. Not exist.");

                return (T)GameObject.Instantiate(prefab);
            }
        }

        public void SetScreen(UI_Screan screen)
        {
            if (_screan != null)
            {
                _screan.Hide();
            }

            _screan = screen;
            _screan.SortingOrder = 0;
        }
    }
}
