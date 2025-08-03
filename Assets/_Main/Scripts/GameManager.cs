using Malgo.GMTK.Player;
using System;
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

        public bool IsGameOver;

        public float CurrentHeat => currentHeat;
        public float CurrentHeatNormalized => (currentHeat - minHeat) / (maxHeat - minHeat);

        public float startTime;

        public static event Action<float> OnGameOver;

        private void Awake()
        {
            currentHeat = minHeat;
        }

        private void Start()
        {
            startTime = Time.time;

            double currentHz = Screen.currentResolution.refreshRateRatio.value;
            Debug.Log(currentHz);
            Application.targetFrameRate = (int) (currentHz);

        }

        private void Update()
        {
            float currentSpeed = playerRB.linearVelocity.magnitude;
            float overSpeed = (currentSpeed / 10f - 1f) * heatIncreaseRate;

            currentHeat = Mathf.Clamp(currentHeat + overSpeed * Time.deltaTime, minHeat, maxHeat);

            if (currentHeat >= maxHeat && !IsGameOver)
            {
                Time.timeScale = 0f;
                OnGameOver?.Invoke(Time.time - startTime);
                IsGameOver = true;
            }
        }
    }
}
