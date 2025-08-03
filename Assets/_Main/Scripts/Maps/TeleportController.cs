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
            Debug.Log(port.isOnDelay);

            if (port == portIn)
            {
                Debug.Log("Tele from " + portIn.name + " to " + portOut.name);

                StartCoroutine(Teleport(player, portOut));
            }
            else if (port == portOut)
            {
                Debug.Log("Tele from " + portOut.name + " to " + portIn.name);
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