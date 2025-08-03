using System;
using System.Collections;
using UnityEngine;

namespace Malgo.GMTK.Player
{
    public class PlayerMovementComponent : MonoBehaviour
    {
        [Header("Movement Stats")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float moveSpeedMultiplier;
        [SerializeField] private float speedLimitStrength;
        [Space]
        [SerializeField] private float groundDrag;

        [Header("Jump")]
        [SerializeField] private float jumpForce;
        private float lastJumpTime;
        private bool canJump = true;
        private bool isJumping = false;

        [Header("Sliding")]
        [SerializeField] private float maxSlideTime;
        [SerializeField] private float slideForce;
        [SerializeField] private float slideYScale;
        private float slideTime;
        private float slideBonusTime;
        private float startYScale;
        private bool isSliding;
        private bool isSlidingBonus;

        [Header("Air Slide Detection")]
        [SerializeField] private float minAirTimeForAirSlide = 0.5f;
        [SerializeField] private float airSlideWindowTime = 0.2f;
        private float airTime;
        private float landingTime;
        private bool wasAirborne;
        private bool isSlidingAirBonus;
        private bool canPerformAirSlide;
        private bool slideInputBuffered;
        private float slideInputTime;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private LayerMask groundLayer;
        private bool isGrounded;
        private bool wasGroundedLastFrame;

        [Header("References")]
        [SerializeField] private Transform mesh;
        [SerializeField] private Transform orientation;
        [SerializeField] private Rigidbody rb;

        // Public properties
        public Vector2 InputVector => inputVector;
        public bool IsSliding => isSliding;
        public float TimeSinceSlide => maxSlideTime - slideTime;
        public float MaxSlideTime => maxSlideTime;

        // Private variables
        private Vector3 moveDirection;
        private Vector2 inputVector;
        private RaycastHit slopeHit;

        public static event Action<bool, float> OnSlideActivated;

        void Start()
        {
            startYScale = mesh.localScale.y;
            wasGroundedLastFrame = true;
        }

        void Update()
        {
            HandleInput();
            SpeedLimit();
            GroundCheck();
            SetJumpState();
            HandleAirSlideDetection();
        }

        private void HandleInput()
        {
            inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

            // Jump input
            if (Input.GetKey(KeyCode.Space))
            {
                TryJump();
            }

            // Sliding input
            if (Input.GetKeyDown(KeyCode.LeftShift) && inputVector != Vector2.zero)
            {
                if (!isSliding)
                {
                    StartSliding();
                }
                else if (!isGrounded && airTime >= minAirTimeForAirSlide)
                {
                    // Buffer the slide input for when player lands
                    slideInputBuffered = true;
                    slideInputTime = Time.time;
                    Debug.Log($"Slide input buffered while airborne! Air time: {airTime:F2}s");
                }
            }

            if (Input.GetKeyUp(KeyCode.LeftShift) && isSliding)
            {
                StopSliding();
            }
        }

        private void HandleAirSlideDetection()
        {
            // Track air time
            if (!isGrounded)
            {
                airTime += Time.deltaTime;
                wasAirborne = true;
            }

            // Clear buffered slide input if too much time has passed
            if (slideInputBuffered && Time.time - slideInputTime > airSlideWindowTime)
            {
                slideInputBuffered = false;
            }

            // Check if player just landed after being airborne
            if (isGrounded && !wasGroundedLastFrame)
            {
                // Player just landed
                landingTime = Time.time;

                // Check if they were airborne long enough to qualify for air slide
                if (wasAirborne && airTime >= minAirTimeForAirSlide)
                {
                    canPerformAirSlide = true;

                    // If slide input was buffered while airborne, execute it immediately
                    if (slideInputBuffered && inputVector != Vector2.zero && !isSliding)
                    {
                        StartSliding(true); // true indicates this is a buffered air slide
                        slideInputBuffered = false;
                        canPerformAirSlide = false;
                    }
                }

                // Reset air time when landing
                airTime = 0f;
                wasAirborne = false;
            }

            // Check if air slide window has expired
            if (canPerformAirSlide && Time.time - landingTime > airSlideWindowTime)
            {
                canPerformAirSlide = false;
            }

            wasGroundedLastFrame = isGrounded;
        }

        private void SpeedLimit()
        {
            Vector3 floatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            float targetMoveSpeed = moveSpeed;

            if (isSliding)
            {
                targetMoveSpeed *= 1.2f;
            }

            if (isSlidingBonus)
            {
                targetMoveSpeed *= 1.5f;
            }

            if (isSlidingAirBonus)
            {
                targetMoveSpeed *= 1.3f;
            }

            if (floatVelocity.magnitude > targetMoveSpeed)
            {
                Vector3 limitedVelocity = floatVelocity.normalized * targetMoveSpeed;
                Vector3 targetVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
                rb.linearVelocity = targetVelocity;
            }
        }

        private void TryJump()
        {
            if (canJump)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

                float jumpForceMultiplier = rb.linearVelocity.magnitude / moveSpeed;

                if (inputVector == Vector2.zero)
                {
                    jumpForceMultiplier = 1f;
                }

                rb.AddForce(Vector3.up * jumpForce * jumpForceMultiplier, ForceMode.VelocityChange);
                isJumping = true;
                lastJumpTime = Time.time;
            }
        }

        private void StartSliding(bool isBufferedAirSlide = false)
        {
            // Check if this is an air slide (either buffered or immediate after landing)
            bool isAirSlide = isBufferedAirSlide || canPerformAirSlide;

            isSliding = true;
            mesh.localScale = new Vector3(mesh.localScale.x, slideYScale, mesh.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            slideTime = maxSlideTime;

            slideBonusTime = UnityEngine.Random.Range(0.7f, maxSlideTime);

            // If this was an air slide, print the message and reset the flag
            if (isAirSlide)
            {
                isSlidingAirBonus = true;
                canPerformAirSlide = false;

                Invoke(nameof(CancelAirSlidingBoost), 1f);
            }

            OnSlideActivated?.Invoke(true, slideBonusTime);
        }

        private void StopSliding()
        {
            isSliding = false;
            mesh.localScale = new Vector3(mesh.localScale.x, startYScale, mesh.localScale.z);

            if (TimeSinceSlide > slideBonusTime - 0.2 && TimeSinceSlide < slideBonusTime)
            {
                isSlidingBonus = true;
                Invoke(nameof(CancelSlidingBoost), 1f);
            }
            OnSlideActivated?.Invoke(false, 0);
        }

        private void CancelSlidingBoost()
        {
            isSlidingBonus = false;
        }

        private void CancelAirSlidingBoost()
        {
            isSlidingAirBonus = false;
        }

        private void GroundCheck()
        {
            isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.1f, groundLayer);
            rb.linearDamping = SetDrag();
        }

        private void SetJumpState()
        {
            bool isOnJumpDelay = Time.time - lastJumpTime < 0.2f;
            canJump = isGrounded && !isJumping && !isOnJumpDelay;
            isJumping = !isGrounded;
        }

        private float SetDrag()
        {
            if (isJumping)
            {
                return 0;
            }
            if (inputVector == Vector2.zero)
            {
                return groundDrag;
            }
            return 0;
        }

        private void FixedUpdate()
        {
            if (isSliding)
            {
                SlidingMovement();
                slideTime -= Time.fixedDeltaTime;
                if (slideTime <= 0f)
                {
                    StopSliding();
                }
            }
            else
            {
                MovePlayer();
            }
        }

        private void MovePlayer()
        {
            moveDirection = orientation.forward * inputVector.y + orientation.right * inputVector.x;
            rb.AddForce(moveDirection.normalized * moveSpeed * Time.deltaTime * moveSpeedMultiplier, ForceMode.Force);
        }

        private void SlidingMovement()
        {
            Vector3 inputDirection = mesh.forward * inputVector.y + mesh.right * inputVector.x;
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
        }
    }
}