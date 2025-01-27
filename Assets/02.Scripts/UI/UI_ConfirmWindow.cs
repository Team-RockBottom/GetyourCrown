using GetyourCrown.UI.UI_Utilities;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_ConfirmWindow : UI_Popup
    {
        [Resolve] TMP_Text _message;
        [Resolve] Button _confirm;

        public void Show(string message, UnityAction onConfirmed = null)
        {
            base.Show();

            _message.text = message;
            _confirm.onClick.RemoveAllListeners();
            _confirm.onClick.AddListener(Hide);

            if (onConfirmed != null)
                _confirm.onClick.AddListener(onConfirmed);
        }
    }
}
