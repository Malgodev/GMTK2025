using System;
using System.Collections;
using UnityEngine;

namespace Malgo.GMTK.Maps
{
    public class TeleportController : MonoBehaviour
    {
        [SerializeField] private TeleportComponent portIn;
        [SerializeField] private TeleportComponent portOut;

        public void RequestTeleport(Transform player, TeleportComponent port)
        {
            if (port == portIn)
            {
                StartCoroutine(Teleport(player, portOut));
            }
            else if (port == portOut)
            {
                StartCoroutine(Teleport(player, portIn));
            }
        }

        private IEnumerator Teleport(Transform player, TeleportComponent otherPort)
        {
            otherPort.SetDelay();
            yield return new WaitForEndOfFrame();
            player.GetComponent<Rigidbody>().MovePosition(otherPort.transform.position);
        }
    }
}