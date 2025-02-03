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
        [Resolve] Toggle _fullScreen;
        [Resolve] Toggle _vSync;
        [Resolve] Button _confirm;
        [Resolve] Button _cancel;

        int currentVSyncCount;
        bool currentFullScreen;
        int prevVSyncCount;
        bool prevIsFullScreen;
        float prevAudioVolum;

        protected override void Start()
        {
            base.Start();

            _volum.onValueChanged.AddListener(delegate { OnSlider(); });

            _fullScreen.onValueChanged.AddListener(delegate { OnFullScreen(); });

            _vSync.onValueChanged.AddListener(delegate { OnVSync(); });

            _confirm.onClick.AddListener(() =>
            {
                VSyncSelect(currentVSyncCount);
                FullScreenSelect(currentFullScreen);
                Hide();
            });

            _cancel.onClick.AddListener(() =>
            {
                VSyncSelect(prevVSyncCount);

                if (prevVSyncCount.Equals(0))
                    _vSync.isOn = false;
                else
                    _vSync.isOn = true;

                FullScreenSelect(prevIsFullScreen);
                _fullScreen.isOn = prevIsFullScreen;

                _volum.value = prevAudioVolum;
                Hide();
            });
        }

        public override void Show()
        {
            base.Show();

            prevVSyncCount = QualitySettings.vSyncCount;
            prevIsFullScreen = _fullScreen.isOn;
            
            prevAudioVolum = _volum.value;
        }

        private void OnVSync()
        {
            if (_vSync.isOn)
                currentVSyncCount = 1;
            else
                currentVSyncCount = 0;
        }

        private void VSyncSelect(int vSync)
        {
            QualitySettings.vSyncCount = vSync;
        }

        private void OnFullScreen()
        {
            if (_fullScreen.isOn)
                currentFullScreen = true;
            else
                currentFullScreen = false;
        }

        private void FullScreenSelect(bool isFullScreen)
        {
            int setWidth = 1920;
            int setHeight = 1080;

            if (isFullScreen)
                Screen.SetResolution(setWidth, setHeight, true);
            else
                Screen.SetResolution(setWidth, setHeight, false);
        }

        private void OnSlider()
        {
            SoundManager.instance.BGMAudioVolum = _volum.value;
            SoundManager.instance.SFXAudioVolum = _volum.value;
            double volumNo = Math.Round(_volum.value, 2);
            _volumSize.text = (volumNo * 100f).ToString();
        }
    }
}
