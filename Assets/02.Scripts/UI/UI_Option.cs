using GetyourCrown.UI.UI_Utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_Option : UI_Popup
    {
        [Resolve] Slider _volum;
        [Resolve] TMP_Text _volumSize;
        [Resolve] Toggle _fullScrean;
        [Resolve] Toggle _vSync;
        [Resolve] Button _confirm;
        [Resolve] Button _cancel;

        protected override void Start()
        {
            base.Start();

            _volum.onValueChanged.AddListener(delegate { OnSlider(); });

            _fullScrean.onValueChanged.AddListener(delegate { OnFullScrean(); });

            _vSync.onValueChanged.AddListener(delegate { OnVSync(); });

            _confirm.onClick.AddListener(() =>
            {
                Hide();
            });

            _cancel.onClick.AddListener(() =>
            {
                Hide();
            });
        }

        public override void Show()
        {
            base.Show();
        }

        private void OnVSync()
        {
            if (_vSync.isOn)
            {
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
            }
        }

        private void OnFullScrean()
        {
            int setWidth = 1920;
            int setHeight = 1080;

            if(_fullScrean.isOn)
            {
                Screen.SetResolution(setWidth, setHeight, true);
            }
            else
            {
                Screen.SetResolution(setWidth, setHeight, false);
            }
        }

        private void OnSlider()
        {
            SoundManager.instance.AudioVolum = _volum.value;
            double volumNo = Math.Round(_volum.value, 2);
            _volumSize.text = (volumNo * 100f).ToString();
        }
    }
}
