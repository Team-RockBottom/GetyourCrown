using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public Transform _parentPosition;

    private bool _isPicking;
    private bool _isPicked;

    Rigidbody _rigidbody;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void PickUp()
    {
        if (_isPicked || _isPicking)
        {
            return;
        }

        _rigidbody.isKinematic = true;
        transform.SetParent(_parentPosition);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        _isPicked = true;
    }
    public void Drop()
    {
        if (!_isPicked)
        {
            return;
        }

        _rigidbody.isKinematic = false;
        transform.SetParent(null);
        _isPicked = false;
    }
}
