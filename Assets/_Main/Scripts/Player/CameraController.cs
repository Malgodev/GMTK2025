using UnityEngine;

namespace Malgo.GMTK.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;

        // Update is called once per frame
        void Update()
        {
            this.transform.position = cameraTransform.position;
        }
    }
}