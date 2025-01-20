using GetyourCrown.UI.UI_Utilities;
using TMPro;
using UnityEngine;
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
            //Debug.Log($"UI_ConfirmWindow.Show() »£√‚µ : {message}");

            _message.text = message;
            _confirm.onClick.RemoveAllListeners();
            _confirm.onClick.AddListener(Hide);

            if (onConfirmed != null)
                _confirm.onClick.AddListener(onConfirmed);
        }
    }
}
