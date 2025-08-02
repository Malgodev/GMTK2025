using System;
using UnityEngine;

namespace Malgo.GMTK.Player
{
    public class PlayerMovementComponent : MonoBehaviour
    {
        [Header("Movement stats")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float moveSpeedMultiplier;

        [Space]
        [SerializeField] private float groundDrag;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private LayerMask groundLayer;
        private bool isGrounded;

        [Header("Jump")]
        [SerializeField] private float jumpForce;
        private float lastJumpTime;
        private bool canJump = true;
        private bool isJumping = false;

        private RaycastHit slopeHit;

        [Header("Other components")]
        [SerializeField] private Transform orientation;

        [Space]
        [SerializeField] private Rigidbody rb;

        Vector3 moveDirection;
        Vector2 inputVector;

        void Start()
        {

        }

        void Update()
        {
            HandleInput();
            SpeedLimit();
            GroundCheck();
            SetJumpState();
        }

        private void HandleInput()
        {
            inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

            if (Input.GetKey(KeyCode.Space))
            {
                TryJump();
            }
        }

        private void SpeedLimit()
        {
            Vector3 floatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            if (floatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = floatVelocity.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
            }
        }

        private void TryJump()
        {
            if (canJump)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                isJumping = true;
                lastJumpTime = Time.time;
            }
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
            MovePlayer();
        }

        private void MovePlayer()
        {
            moveDirection = orientation.forward * inputVector.y + orientation.right * inputVector.x;

            rb.AddForce(moveDirection.normalized * moveSpeed * Time.deltaTime * moveSpeedMultiplier, ForceMode.Force);
        }
    }
}