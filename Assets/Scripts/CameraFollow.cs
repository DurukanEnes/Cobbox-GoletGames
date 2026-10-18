using UnityEngine;

namespace Cobbox
{
    // Follows the ball smoothly, keeping the offset the camera had in the scene. Stops on game over.
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] float smoothTime = 0.15f;

        Vector3 offset;
        Vector3 velocity;

        void Start()
        {
            offset = transform.position - target.position;
        }

        void LateUpdate()
        {
            if (GameManager.Instance.State == GameState.GameOver) return;

            Vector3 desired = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }
    }
}
