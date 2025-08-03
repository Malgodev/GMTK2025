using UnityEngine;


namespace Malgo.GMTK.Maps
{
    public class HeatUIController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        [SerializeField] private SlicedFilledImage heatBar;
        [SerializeField] private Gradient heatGradient;

        private void Update()
        {
            heatBar.fillAmount = gameManager.CurrentHeatNormalized;
            heatBar.color = heatGradient.Evaluate(gameManager.CurrentHeatNormalized);
        }
    }
}