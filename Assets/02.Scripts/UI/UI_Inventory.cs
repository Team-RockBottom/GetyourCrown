using UnityEngine;
using UnityEngine.InputSystem;

namespace Practices.UGUI_Management.UI
{
    public class UI_Inventory : UI_Popup
    {
        protected override void Start()
        {
            base.Start();

            playerInputActions.UI.Click.performed += OnClick;
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            if (TryGraphicRaycast(Mouse.current.position.ReadValue(), out CanvasRenderer renderer))
            {
                UI_ConfirmWindow confirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                confirmWindow.Show("아직 인벤토리 구현된 사항 없음");
            }
        }
    }
}