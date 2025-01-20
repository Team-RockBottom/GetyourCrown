using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Practices.UGUI_Management.Utilities;

namespace Practices.UGUI_Management.UI
{
    public class UI_Popup : UI_Base
    {
        [Resolve] Image _panel;
        private bool _onDragging;


        protected override void Start()
        {
            base.Start();

            playerInputActions.UI.Click.performed += CheckOtherUIClicked;
            playerInputActions.UI.RightClick.performed += CheckOtherUIClicked;
            playerInputActions.UI.Drag.performed += OnDrag;
        }

        void CheckOtherUIClicked(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton() == false)
                return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // 일단 이 Canvas 에서 뭔갈 클릭했는지 확인
            if (TryGraphicRaycast(mousePosition, out CanvasRenderer renderer))
            {
                // Nothing todo
            }
            else
            {
                IEnumerable<UI_Popup> popups = manager.popups;

                foreach (UI_Popup popup in popups)
                {
                    if (popup == this)
                        continue;

                    // 유저가 다른 팝업 건드림. 건드린 팝업을 최상단으로 보내줌.
                    if (popup.TryGraphicRaycast(mousePosition, out renderer))
                    {
                        popup.Show();
                        break;
                    }
                }
            }
        }

        void OnDrag(InputAction.CallbackContext context)
        {
            if (TryGraphicRaycast(Mouse.current.position.ReadValue(), out Image result))
            {
                if (result == _panel)
                {
                    StartCoroutine(C_OnDrag(context, Mouse.current.position.ReadValue() - (Vector2)_panel.transform.position));
                }
            }
        }

        IEnumerator C_OnDrag(InputAction.CallbackContext context, Vector2 offset)
        {
            while (context.action.ReadValue<Vector2>().magnitude > 0)
            {
                _panel.transform.position = Mouse.current.position.ReadValue() + offset;
                yield return null;
            }
        }
    }
}