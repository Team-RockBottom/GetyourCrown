using GetyourCrown.UI.UI_Utilities;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_ConfirmWindow : UI_Popup
    {
        public bool ConfirmInteractable
        {
            get => _confirmValue;
            set
            {
                if (value)
                {
                    _confirmValue = true;
                    _confirm.interactable = true;
                }
                else
                {
                    _confirmValue = false;
                    _confirm.interactable = false;
                }
            }
        }

        bool _confirmValue;
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
