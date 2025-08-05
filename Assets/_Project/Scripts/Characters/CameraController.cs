using UnityEngine;
using System.Collections.Generic;

namespace Platformer
{
    /// <summary>
    /// Manages the game camera by following a target smoothly.
    /// It uses a list of fixed camera positions (offsets) that the player
    /// can cycle through using an input action.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("The transform the camera will follow. This should be the player.")]
        [SerializeField] private Transform target;

        [Header("References")]
        [Tooltip("The Input Reader asset that broadcasts input events.")]
        [SerializeField] private InputReader inputReader;

        [Header("Settings")]
        [Tooltip("A list of camera positions relative to the target. You can add as many as you want.")]
        [SerializeField] private List<Vector3> offsets = new List<Vector3> { new Vector3(0, 5, -10) };
        
        [Tooltip("How quickly the camera moves to its target position. Smaller values are faster.")]
        [SerializeField] private float positionSmoothTime = 0.125f;

        // Private variables to manage state.
        private int _currentOffsetIndex = 0;
        private Vector3 _currentPositionVelocity = Vector3.zero;

        private void OnEnable()
        {
            if (inputReader != null)
            {
                // Subscribe to the camera cycle event when this component is enabled.
                inputReader.CycleCameraEvent += HandleCycleCamera;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                // It's crucial to unsubscribe when the component is disabled to prevent errors.
                inputReader.CycleCameraEvent -= HandleCycleCamera;
            }
        }

        /// <summary>
        /// This method is called when the CycleCameraEvent is invoked by the InputReader.
        /// It moves to the next camera position in the list.
        /// </summary>
        private void HandleCycleCamera()
        {
            _currentOffsetIndex++;

            // If we've gone past the end of the list, loop back to the beginning.
            if (_currentOffsetIndex >= offsets.Count)
            {
                _currentOffsetIndex = 0;
            }
            Debug.Log($"Camera view changed to offset #{_currentOffsetIndex + 1}");
        }
        
        /// <summary>
        /// LateUpdate is called after all Update functions have been called.
        /// This is the best place to move a camera to ensure the target has finished its movement for the frame.
        /// </summary>
        private void LateUpdate()
        {
            if (target == null || offsets.Count == 0) return;

            // Calculate the camera's desired position based on the target and the current offset.
            Vector3 desiredPosition = target.position + offsets[_currentOffsetIndex];
            
            // Use SmoothDamp for frame-rate independent, smooth camera movement.
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentPositionVelocity, positionSmoothTime);
            
            // Make the camera always look at the player's position.
            transform.LookAt(target.position + Vector3.up * 1.5f);
        }

        /// <summary>
        /// Allows other scripts to set the camera's target at runtime.
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
