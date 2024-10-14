using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Assets.Scripts
{
    public class SwipeDetector : MonoBehaviour
    {
        private Vector2 _startPosition;
        private Vector2 _endPosition;
        private bool _swipeDetected;
        private InputAction _touchAction;
        private InputAction _mouseAction;

        [SerializeField] private float minSwipeDistance = 0.2f;

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
                }
                else if (touch.phase.ReadValue() == TouchPhase.Moved)
                {
                    _endPosition = touch.position.ReadValue();
                    DetectSwipe();
                }
            }
        }

        private void OnTouchCanceled(InputAction.CallbackContext context)
        {
            if (context.control is TouchControl touch && touch.phase.ReadValue() == TouchPhase.Ended)
            {
                _endPosition = touch.position.ReadValue();
                DetectSwipe();
            }
        }

        private void OnMousePerformed(InputAction.CallbackContext context)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _startPosition = Mouse.current.position.ReadValue();
                _swipeDetected = false;
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                _endPosition = Mouse.current.position.ReadValue();
                DetectSwipe();
            }
        }

        private void OnMouseCanceled(InputAction.CallbackContext context)
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _endPosition = Mouse.current.position.ReadValue();
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
                Vector2 direction = swipeVector.normalized;
                Debug.Log($"Swipe detected! Direction: {direction}");
            }
        }
    }
}
