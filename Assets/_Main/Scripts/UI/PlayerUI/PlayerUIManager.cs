using TMPro;
using UnityEngine;

namespace Malgo.GMTK.UI
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerSpeed;
        [SerializeField] private Rigidbody playerRigidbody;

        private void Update()
        {
            playerSpeed.text = $"{playerRigidbody.linearVelocity.magnitude:F2}";
        }
    }
}