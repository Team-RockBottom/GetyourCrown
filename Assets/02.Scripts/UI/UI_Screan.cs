using UnityEngine;

namespace GetYourCrown.UI
{
    public class UI_Screan : UI_Base
    {
        public override void Show()
        {
            base.Show();

            _manager.SetScreen(this);
        }
    }
}
