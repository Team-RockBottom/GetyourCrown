using UnityEngine;

public class KickableObject : MonoBehaviour
{
    Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Kick(Vector3 force)
    {
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }
}
