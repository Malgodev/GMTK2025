using System;
using UnityEngine;

namespace Malgo.GMTK.Maps
{
    public class MapComponent : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private MapController mapController;

        private float checkInterval = 0.1f;

        public void Init(Transform player, MapController mapController)
        {
            this.player = player;
            this.mapController = mapController;
        }

        //private void OnEnable()
        //{
        //    checkInterval = UnityEngine.Random.Range(0.05f, 0.2f);
        //    InvokeRepeating(nameof(CheckPlayerPosition), 0f, checkInterval);
        //}

        //private void CheckPlayerPosition()
        //{
        //    if (Vector3.Distance(player.position, transform.position) > 150f)
        //    {
        //        mapController.ReturnMap(this);
        //    }

        //}

        //private void OnDisable()
        //{
        //    CancelInvoke(nameof(CheckPlayerPosition));
        //}
    }
}