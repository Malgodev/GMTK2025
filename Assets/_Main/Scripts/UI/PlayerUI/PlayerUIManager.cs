using Malgo.GMTK.Player;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Malgo.GMTK.UI
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerSpeed;
        [SerializeField] private Rigidbody playerRigidbody;
        [SerializeField] private PlayerMovementComponent playerMovementComponent;

        [Space]
        [SerializeField] private Image slidingImage;
        [SerializeField] private Image slidingBoostRange;

        private void OnEnable()
        {
            PlayerMovementComponent.OnSlideActivated += UpdateSlidingUI;
        }

        private void UpdateSlidingUI(bool isSliding, float randomBoost)
        {
            if (isSliding)
            {
                slidingBoostRange.gameObject.SetActive(true);

                slidingBoostRange.transform.rotation = Quaternion.Euler(0, 0, -(randomBoost * 360f - 72f));

                StartCoroutine(StartSliding(playerMovementComponent.MaxSlideTime));
            }
            else
            {
                slidingImage.fillAmount = 0f;
                slidingBoostRange.gameObject.SetActive(false);
            }
        }

        IEnumerator StartSliding(float maxSlideTime)
        {
            while (playerMovementComponent.IsSliding)
            {
                slidingImage.fillAmount = playerMovementComponent.TimeSinceSlide / maxSlideTime;
                yield return null;
            }
        }

        private void Update()
        {
            playerSpeed.text = $"{playerRigidbody.linearVelocity.magnitude:F2}";
        }

        private void OnDisable()
        {
            PlayerMovementComponent.OnSlideActivated -= UpdateSlidingUI;
        }
    }
}