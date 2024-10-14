using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class SwipeDetector : MonoBehaviour
    {
        private Vector2 _startPosition;
        private Vector2 _endPosition;
        private bool _swipeDetected;
        private InputAction _touchAction;
        private InputAction _mouseAction;
        private List<GameObject> _swipeObjects;

        [SerializeField] private float minSwipeDistance = 0.2f;
        [SerializeField] private float swipeSpeed = 1.0f;
        [SerializeField] private ThrowableObject throwableObjectPrefab;
        private ThrowableObject _currentThrowableObject;

        private void OnEnable()
        {
            _touchAction = new InputAction(binding: "<Touchscreen>/primaryTouch");
            _touchAction.performed += ctx => OnTouchPerformed(ctx);
            _touchAction.canceled += ctx => OnTouchCanceled(ctx);
            _touchAction.Enable();

            _mouseAction = new InputAction(binding: "<Mouse>/leftButton");
            _mouseAction.performed += ctx => OnMousePerformed(ctx);
            _mouseAction.canceled += ctx => OnMouseCanceled(ctx);
            _mouseAction.Enable();

            _swipeObjects = new List<GameObject>();
        }

        private void OnDisable()
        {
            _touchAction.Disable();
            _touchAction.Dispose();

            _mouseAction.Disable();
            _mouseAction.Dispose();
        }

        private void OnTouchPerformed(InputAction.CallbackContext context)
        {
            if (context.control is TouchControl touch)
            {
                if (touch.phase.ReadValue() == TouchPhase.Began)
                {
                    _startPosition = touch.position.ReadValue();
                    _swipeDetected = false;
                    CreateSwipeObject();
                }
                else if (touch.phase.ReadValue() == TouchPhase.Moved)
                {
                    _endPosition = touch.position.ReadValue();
                    MoveSwipeObject();
                    DetectSwipe();
                }
            }
        }

        private void OnTouchCanceled(InputAction.CallbackContext context)
        {
            if (context.control is TouchControl touch && touch.phase.ReadValue() == TouchPhase.Ended)
            {
                _endPosition = touch.position.ReadValue();
                //InstantiateSwipeObject();
                DetectSwipe();
            }
        }

        private void OnMousePerformed(InputAction.CallbackContext context)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _startPosition = Mouse.current.position.ReadValue();
                _swipeDetected = false;
                CreateSwipeObject();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                _endPosition = Mouse.current.position.ReadValue();
                MoveSwipeObject();
                DetectSwipe();
            }
        }

        private void OnMouseCanceled(InputAction.CallbackContext context)
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _endPosition = Mouse.current.position.ReadValue();
                //InstantiateSwipeObject();
                DetectSwipe();
            }
        }

        private void DetectSwipe()
        {
            if (_swipeDetected)
            {
                return;
            }

            Vector2 swipeVector = _endPosition - _startPosition;
            if (swipeVector.magnitude >= minSwipeDistance)
            {
                _swipeDetected = true;

                Vector3 throwDirection = new Vector3(((Vector2)swipeVector).x, 0, ((Vector2)swipeVector).y); 
                Debug.Log($"Swipe detected! Direction: {throwDirection}");

                InstantiateSwipeObject(throwDirection);
            }
        }

        private void CreateSwipeObject()
        {
            _currentThrowableObject = Instantiate(throwableObjectPrefab);
            _currentThrowableObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(_startPosition.x, _startPosition.y, 10f));
            Debug.Log("Swipe object created!");
        }

        private void MoveSwipeObject()
        {
            if (_currentThrowableObject != null)
            {
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(_endPosition.x, _endPosition.y, 10f));
                _currentThrowableObject.transform.position = newPosition;
                Debug.Log("Swipe object moved!");
            }
        }

        private void InstantiateSwipeObject(Vector3 throwDirection)
        {
            if (_currentThrowableObject != null)
            {
                _currentThrowableObject.Throw(throwDirection, 1f); 
                _swipeObjects.Add(_currentThrowableObject.gameObject);
                _currentThrowableObject = null;
                Debug.Log("Swipe object instantiated!");
            }
        }
    }
}
