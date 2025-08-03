using TMPro;
using UnityEngine;


namespace Malgo.GMTK.Maps
{
    public class HeatUIController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        [SerializeField] private SlicedFilledImage heatBar;
        [SerializeField] private Gradient heatGradient;

        [SerializeField] private TMP_Text timer;

        private void Update()
        {
            timer.text = $"{Mathf.FloorToInt(Time.time - gameManager.startTime)}";

            heatBar.fillAmount = gameManager.CurrentHeatNormalized;
            heatBar.color = heatGradient.Evaluate(gameManager.CurrentHeatNormalized);
        }
    }
}