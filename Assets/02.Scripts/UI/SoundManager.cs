using System.Collections.Generic;
using UnityEngine;

namespace GetyourCrown.UI
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;

        public float BGMAudioVolum
        {
            get => _bgmAudioSource.volume;
            set => _bgmAudioSource.volume = value;
        }

        public float SFXAudioVolum
        {
            get => _sfxAudioSource.volume;
            set => _sfxAudioSource.volume = value;
        }

        [SerializeField] AudioSource _bgmAudioSource;
        [SerializeField] AudioSource _sfxAudioSource;

        Dictionary<string, AudioClip> _bgmClips = new Dictionary<string, AudioClip>();
        Dictionary<string, AudioClip> _sfxClips = new Dictionary<string, AudioClip>();

        [System.Serializable]
        public struct NamedAudioClip
        {
            public string name;
            public AudioClip clip;
        }

        [SerializeField] NamedAudioClip[] BGMClipList;
        [SerializeField] NamedAudioClip[] SFXClipList;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            InitializedAudioClips();
        }

        void InitializedAudioClips()
        {
            foreach (var bgm in BGMClipList)
            {
                if (!_bgmClips.ContainsKey(bgm.name))
                {
                    _bgmClips.Add(bgm.name, bgm.clip);
                }
            }
            foreach (var sfx in SFXClipList)
            {
                if (!_sfxClips.ContainsKey(sfx.name))
                {
                    _sfxClips.Add(sfx.name, sfx.clip);
                }
            }
        }

        public void PlayBGM(string name)
        {
            if (_bgmClips.ContainsKey(name))
            {
                _bgmAudioSource.clip = _bgmClips[name];
                _bgmAudioSource.Play();
            }
            else
            {
                Debug.LogError($"BGM : {name}가 없습니다.");
            }
        }

        public void PlaySFX(string name, Vector3 position)
        {
            if (_sfxClips.ContainsKey(name))
            {
                AudioSource.PlayClipAtPoint(_sfxClips[name], position);
            }
            else
            {
                Debug.LogError($"SFX : {name}가 없습니다.");
            }
        }

        public void StopBGM()
        {
            _bgmAudioSource.Stop();
        }

        public void StopSFX()
        {
            _sfxAudioSource.Stop();
        }
    }
}
