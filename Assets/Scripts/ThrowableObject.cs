using UnityEngine;

namespace Assets.Scripts
{
    public class ThrowableObject : MonoBehaviour
    {
        [SerializeField] private float _destroyTime = 3f;
        [SerializeField] private Rigidbody _rigidbody;

        public void Throw(Vector3 direction, float throwForce)
        {
            _rigidbody.AddForce(direction * throwForce, ForceMode.Force);
            Destroy(gameObject, _destroyTime);
        }
    }
}