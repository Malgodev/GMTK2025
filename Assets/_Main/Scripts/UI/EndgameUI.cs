using Malgo.Utilities.UI;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Malgo.GMTK.UI
{
    public class EndgameUI : MonoBehaviour
    {
        [SerializeField] private UIAnimation endgameAnimation;

        [SerializeField] private TMP_Text time;
        [SerializeField] private Button retryButton;

        private void OnEnable()
        {
            GameManager.OnGameOver += ShowEndgameUI;
        }

        private void Start()
        {
        }

        private void ShowEndgameUI(float obj)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            time.text = $"You escaped the simualtion after {obj:F2} seconds!";
            endgameAnimation.gameObject.SetActive(true);

            retryButton.interactable = true;
            retryButton.gameObject.SetActive(true);
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(Retry);
        }

        private void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            Time.timeScale = 1f;
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= ShowEndgameUI;
        }
    }
}
