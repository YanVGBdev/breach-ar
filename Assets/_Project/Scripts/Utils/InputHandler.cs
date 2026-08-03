using UnityEngine;
using System;
using VContainer;

namespace BreachAR.Utils
{
    /// <summary>
    /// Unified input handler for touch and mouse
    /// </summary>
    public class InputHandler : MonoBehaviour
    {

        [Header("Settings")]
        [SerializeField] private bool enableInput = true;
        [SerializeField] private float longPressDuration = 0.5f;

        // Input state
        private bool isInputDown;
        private bool isInputPressed;
        private bool isInputUp;
        private Vector2 inputPosition;
        private Vector2 inputStartPosition;
        private float inputDownTime;
        private bool isLongPress;

        // Events
        public event Action<Vector2> OnInputDown;
        public event Action<Vector2> OnInputPressed;
        public event Action<Vector2> OnInputUp;
        public event Action<Vector2> OnTap;
        public event Action<Vector2> OnDoubleTap;
        public event Action<float> OnLongPress;

        public bool IsInputDown => isInputDown;
        public bool IsInputPressed => isInputPressed;
        public bool IsInputUp => isInputUp;
        public Vector2 InputPosition => inputPosition;
        public Vector2 InputStartPosition => inputStartPosition;
        public Vector2 InputDelta => inputPosition - (Vector2)inputStartPosition;
        public float InputHoldDuration => Time.time - inputDownTime;
        public bool IsLongPress => isLongPress;



        private void Update()
        {
            if (!enableInput) return;

            ResetInputFlags();
            HandleInput();
        }

        private void ResetInputFlags()
        {
            isInputDown = false;
            isInputUp = false;
        }

        private void HandleInput()
        {
            // Touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                inputPosition = touch.position;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnInputBegan(touch.position);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        OnInputMoved(touch.position);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        OnInputEnded(touch.position);
                        break;
                }
            }
            // Mouse input (editor/standalone)
            else if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                inputPosition = Input.mousePosition;

                if (Input.GetMouseButtonDown(0))
                {
                    OnInputBegan(Input.mousePosition);
                }
                else if (Input.GetMouseButton(0))
                {
                    OnInputMoved(Input.mousePosition);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    OnInputEnded(Input.mousePosition);
                }
            }

            // Check for long press
            if (isInputPressed && !isLongPress)
            {
                if (InputHoldDuration >= longPressDuration)
                {
                    isLongPress = true;
                    OnLongPress?.Invoke(InputHoldDuration);
                }
            }
        }

        private void OnInputBegan(Vector2 position)
        {
            isInputDown = true;
            isInputPressed = true;
            inputStartPosition = position;
            inputDownTime = Time.time;
            isLongPress = false;

            OnInputDown?.Invoke(position);
        }

        private void OnInputMoved(Vector2 position)
        {
            OnInputPressed?.Invoke(position);
        }

        private void OnInputEnded(Vector2 position)
        {
            isInputPressed = false;
            isInputUp = true;

            OnInputUp?.Invoke(position);

            // Check for tap (short press)
            if (!isLongPress && InputHoldDuration < longPressDuration)
            {
                float distance = Vector2.Distance(position, inputStartPosition);
                if (distance < 50f) // Tap threshold
                {
                    OnTap?.Invoke(position);
                }
            }
        }

        /// <summary>
        /// Enable/disable input
        /// </summary>
        public void SetInputEnabled(bool enabled)
        {
            enableInput = enabled;
        }

        /// <summary>
        /// Get normalized input position (0-1)
        /// </summary>
        public Vector2 GetNormalizedPosition()
        {
            return new Vector2(
                inputPosition.x / Screen.width,
                inputPosition.y / Screen.height
            );
        }

        /// <summary>
        /// Check if input is in specific screen region
        /// </summary>
        public bool IsInRegion(Rect region)
        {
            return region.Contains(inputPosition);
        }
    }
}
