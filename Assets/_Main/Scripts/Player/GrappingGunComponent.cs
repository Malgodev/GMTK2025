using DG.Tweening;
using System;
using UnityEngine;

namespace Malgo.GMTK.Player
{
    public class GrapplingGunComponent : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        private Vector3 grapplePoint;
        private Transform cam;

        [Header("Transforms")]
        [SerializeField] private Transform gunTip;
        [SerializeField] private Transform player;
        private Rigidbody playerRB;

        [Header("Grapple Stat")]
        [SerializeField] public LayerMask groundMask;
        [SerializeField] private float maxDistance = 100f;
        private SpringJoint joint;
        private RaycastHit grappleHit;
        [SerializeField] private float springMaxDistance = 0.8f;
        [SerializeField] private float springMinDistance = 0.25f;

        [Header("Grapple Drag")]
        [SerializeField] private float dragPower = 10f;
        [SerializeField] private float stopDragDistance = 2f; // Stop dragging when this close

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            cam = Camera.main.transform;
            playerRB = player.GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShootGrapple();
            }
            if (Input.GetMouseButtonUp(0))
            {
                StopGrapple();
            }
            if (Input.GetMouseButton(1))
            {
                MoveToGrapplePoint();
            }
        }

        private void MoveToGrapplePoint()
        {
            if (!joint) return;

            Vector3 direction = grapplePoint - player.position;
            float distanceToTarget = direction.magnitude;

            // Stop applying force when close enough to prevent jiggling
            if (distanceToTarget > stopDragDistance)
            {
                // Normalize the direction and apply force
                direction = direction.normalized;

                // Optional: Reduce force as we get closer to create smoother movement
                float forceMagnitude = dragPower * Mathf.Clamp01(distanceToTarget / 10f);
                playerRB.AddForce(direction * forceMagnitude, ForceMode.Acceleration);
            }
        }

        private void ShootGrapple()
        {
            if (Physics.Raycast(cam.position, cam.forward, out grappleHit, maxDistance, groundMask))
            {
                // Fix: Assign the grapple point from the hit
                grapplePoint = grappleHit.point;

                joint = player.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint; // Now this will work correctly

                float distance = Vector3.Distance(player.position, grapplePoint);
                joint.maxDistance = distance * springMaxDistance;
                joint.minDistance = distance * springMinDistance;
                joint.spring = 4.5f;
                joint.damper = 7f;
                joint.massScale = 4.5f;
            }
        }

        private void LateUpdate()
        {
            DrawRope();
        }

        private void DrawRope()
        {
            if (!joint) return;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, gunTip.position);
            lineRenderer.SetPosition(1, grapplePoint); // Use grapplePoint for consistency
        }

        private void StopGrapple()
        {
            lineRenderer.positionCount = 0;
            Destroy(joint);
        }
    }
}