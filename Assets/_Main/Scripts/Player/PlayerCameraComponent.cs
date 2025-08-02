using UnityEngine;

namespace Malgo.GMTK.Player
{
    public class PlayerCameraComponent : MonoBehaviour
    {
        [SerializeField] private Transform orientation;

        [Header("Settings")]
        [SerializeField] private float sensX;
        [SerializeField] private float sensY;


        float xRotation;
        float yRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensX * Time.deltaTime;
        
            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);


            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
}