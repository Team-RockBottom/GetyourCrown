    using GetyourCrown.UI.UI_Utilities;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;

    namespace GetyourCrown.UI
    {
        public class UI_Popup : UI_Base
        {
            [Resolve] Image _panel;

            protected override void Start()
            {
                base.Start();

            playerInputActions.UI.Click.performed += MoveUITop;
        }

            void MoveUITop(InputAction.CallbackContext context)
            {
                if (context.ReadValueAsButton() == false)
                    return;

                Vector2 mousePos = Mouse.current.position.ReadValue();

                if (TryGraphicRaycast(mousePos, out CanvasRenderer renderer))
                {

                }
                else
                {
                    IEnumerable<UI_Popup> popups = manager.popups;

                    foreach (UI_Popup popup in popups)
                    {
                        if (popup == this)
                            continue;

                        if (popup.TryGraphicRaycast(mousePos, out renderer))
                        {
                            popup.Show();
                            break;
                        }
                    }
                }
            }
        }
    }
