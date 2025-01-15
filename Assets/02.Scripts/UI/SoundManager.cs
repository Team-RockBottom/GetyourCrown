using UnityEngine;

namespace GetyourCrown.UI
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;

        public float AudioVolum
        {
            get => _audioSource.volume;
            set => _audioSource.volume = value;
        }

        AudioSource _audioSource;

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
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }
}
