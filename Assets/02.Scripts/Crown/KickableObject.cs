using GetyourCrown.UI;
using UnityEngine;

namespace Crown
{
    public class KickableObject : MonoBehaviour
    {
        Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Kick(Vector3 force)
        {
            SoundManager.instance.PlaySFX("KickingSound", transform.position);
            Debug.Log("call kick");
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}
