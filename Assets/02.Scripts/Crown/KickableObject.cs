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
            Debug.Log("call kick");
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}
