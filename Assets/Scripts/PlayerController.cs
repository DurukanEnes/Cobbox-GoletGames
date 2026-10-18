using UnityEngine;
using UnityEngine.InputSystem;

namespace Cobbox
{
    // Moves the ball, switches direction on tap, flies up while the screen is held
    // on a jump-zone tile, and handles falling. Vertical movement is done by hand (no Rigidbody).
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] float startSpeed = 4f;
        [SerializeField] float maxSpeed = 9f;
        [SerializeField] float acceleration = 0.1f;

        [Header("Jump")]
        [SerializeField] float riseSpeed = 6f;
        [SerializeField] float maxRise = 2.5f;

        [Header("Falling")]
        [SerializeField] float gravity = 25f;
        [SerializeField] float groundCheckDistance = 0.1f;
        [SerializeField] float fallTolerance = 0.5f;
        [SerializeField] LayerMask groundMask = ~0;

        public float Speed { get; private set; }
        public Vector3 Direction { get; private set; } = Vector3.forward;
        public bool IsGrounded { get; private set; }
        public bool IsJumping { get; private set; }
        public Tile CurrentTile { get; private set; }

        float radius;
        float verticalVelocity;
        float lastGroundY;
        float takeoffY;
        bool hitWall;

        void Awake()
        {
            radius = GetComponent<SphereCollider>().radius * transform.lossyScale.y;
            Speed = startSpeed;
            lastGroundY = transform.position.y - radius;
            IsGrounded = true;
        }

        void Update()
        {
            var game = GameManager.Instance;

            if (PressedThisFrame())
                OnPress(game);

            if (game.State == GameState.Ready) return;

            if (game.State == GameState.Playing)
                Speed = Mathf.Min(maxSpeed, Speed + acceleration * Time.deltaTime);

            MoveForward(game);
            UpdateVertical(game);
        }

        void OnPress(GameManager game)
        {
            switch (game.State)
            {
                case GameState.Ready:
                    game.StartGame();
                    break;
                case GameState.Playing:
                    if (!IsGrounded) break;
                    if (CurrentTile != null && CurrentTile.IsJumpZone)
                        StartJump();
                    else
                        Direction = Direction == Vector3.forward ? Vector3.right : Vector3.forward;
                    break;
                case GameState.GameOver:
                    if (game.CanRestart) game.Restart();
                    break;
            }
        }

        void StartJump()
        {
            IsJumping = true;
            IsGrounded = false;
            takeoffY = transform.position.y;
            verticalVelocity = riseSpeed;
        }

        void MoveForward(GameManager game)
        {
            if (hitWall) return;

            // Hitting the side of a higher pillar ends the run.
            float step = Speed * Time.deltaTime;
            if (Physics.Raycast(transform.position, Direction, radius + step, groundMask, QueryTriggerInteraction.Ignore))
            {
                hitWall = true;
                game.GameOver();
                return;
            }

            transform.position += Direction * step;
        }

        void UpdateVertical(GameManager game)
        {
            Vector3 position = transform.position;
            bool hitGround = Physics.Raycast(position, Vector3.down, out RaycastHit hit,
                radius + groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);

            if (hitGround && verticalVelocity <= 0f)
            {
                IsGrounded = true;
                IsJumping = false;
                verticalVelocity = 0f;
                position.y = hit.point.y + radius;
                lastGroundY = hit.point.y;
                EnterTile(hit.collider.GetComponentInParent<Tile>(), game);
            }
            else
            {
                IsGrounded = false;

                // Keep rising while the screen is held, up to maxRise; after release the ball just falls.
                if (IsJumping && (!HeldDown() || position.y - takeoffY >= maxRise))
                    IsJumping = false;

                if (IsJumping)
                    verticalVelocity = riseSpeed;
                else
                    verticalVelocity -= gravity * Time.deltaTime;

                position.y += verticalVelocity * Time.deltaTime;

                if (position.y - radius < lastGroundY - fallTolerance)
                    game.GameOver();
            }

            transform.position = position;

            // Stop updating once the ball is far below the path.
            if (position.y < lastGroundY - 30f)
                enabled = false;
        }

        void EnterTile(Tile tile, GameManager game)
        {
            if (tile == null || tile == CurrentTile) return;

            if (CurrentTile != null)
            {
                CurrentTile.Drop();
                game.AddScore(1);
            }
            CurrentTile = tile;
        }

        // Pointer covers both touch (phone) and mouse (Editor).
        static bool PressedThisFrame()
        {
            var pointer = Pointer.current;
            return pointer != null && pointer.press.wasPressedThisFrame;
        }

        static bool HeldDown()
        {
            var pointer = Pointer.current;
            return pointer != null && pointer.press.isPressed;
        }
    }
}
