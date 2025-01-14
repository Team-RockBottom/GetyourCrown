using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public Transform _parentPosition;

    private bool _isPicking = false;
    private bool _isPicked = false;

    Collider _collider;
    Rigidbody _rigidbody;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
    }

    public void PickUp()
    {
        Debug.Log("Call PickUp");

        if (_isPicked || _isPicking)
        {
            return;
        }

        _rigidbody.isKinematic = true;
        transform.SetParent(_parentPosition);
        transform.position = _parentPosition.position;
        transform.rotation = _parentPosition.rotation;
        _collider.isTrigger = true;
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
