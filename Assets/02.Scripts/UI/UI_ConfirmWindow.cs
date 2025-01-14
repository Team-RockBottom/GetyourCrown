using GetyourCrown.UI.UI_Utilities;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_ConfirmWindow : UI_Popup
    {
        [Resolve] TMP_Text _message;
        [Resolve] Button _button;

        public void Show(string message, UnityAction onConfirmed = null)
        {
            base.Show();

            _message.text = message;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(Hide);

            if (onConfirmed != null)
                _button.onClick.AddListener(onConfirmed);
        }
    }
}
