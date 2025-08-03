using Malgo.GMTK.Player;
using UnityEngine;

namespace Malgo.GMTK
{
    public class GameManager : MonoBehaviour
    {
        private float currentHeat;
        [SerializeField] private float minHeat = 60f;
        [SerializeField] private float maxHeat = 120f;
        [SerializeField] private float heatIncreaseRate = 1f;

        [SerializeField] private Rigidbody playerRB;

        public float CurrentHeat => currentHeat;
        public float CurrentHeatNormalized => (currentHeat - minHeat) / (maxHeat - minHeat);

        private void Awake()
        {
            currentHeat = minHeat;
        }

        private void Update()
        {
            float currentSpeed = playerRB.linearVelocity.magnitude;
            float overSpeed = (currentSpeed / 10f - 1f) * heatIncreaseRate;

            Debug.Log($"Current Speed: {currentSpeed}, Over Speed: {overSpeed}");

            currentHeat = Mathf.Clamp(currentHeat + overSpeed * Time.deltaTime, minHeat, maxHeat);
        }
    }
}
