using System;
using UnityEngine;

namespace Malgo.GMTK.Maps
{
    public class TeleportComponent : MonoBehaviour
    {
        private float delayDuration = 0.5f;
        public bool isOnDelay;

        [SerializeField] private TeleportController pairedPort;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !isOnDelay)
            {
                isOnDelay = true;
                Invoke(nameof(ResetDelay), delayDuration);
                pairedPort.RequestTeleport(other.transform.parent, this);
            }
        }

        private void ResetDelay()
        {
            isOnDelay = false;
        }

        public void SetDelay()
        {
            isOnDelay = true;
            Invoke(nameof(ResetDelay), delayDuration);
        }
    }
}