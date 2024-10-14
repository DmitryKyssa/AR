using UnityEngine;

namespace Assets.Scripts
{
    public class ThrowableObject : MonoBehaviour
    {
        [SerializeField] private float _destroyTime = 30f;
        [SerializeField] private Rigidbody _rigidbody;

        public void Throw(Vector3 direction, float throwForce)
        {
            _rigidbody.AddForce(direction * throwForce, ForceMode.Impulse);
            Destroy(gameObject, _destroyTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("Collided with " + other.gameObject.name);
            _rigidbody.AddForce(Vector3.up, ForceMode.Impulse);
            //Destroy(gameObject);
        }
    }
}