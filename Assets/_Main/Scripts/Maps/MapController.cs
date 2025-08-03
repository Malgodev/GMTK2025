using Malgo.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Malgo.GMTK.Maps
{
    public class MapController : MonoBehaviour
    {
        [Header("Map Stat")]
        [SerializeField] private float mapRenderDistance = 150f;
        [SerializeField] private float mapSpacing = 50f; // Distance between maps

        [Header("Transform")]
        [SerializeField] private Transform mapPool;
        [SerializeField] private Transform activeMapParent;
        [SerializeField] private Transform playerTransform;

        [Header("Fog Walls")]
        [SerializeField] private Transform minZWall;
        [SerializeField] private Transform maxZWall;
        [SerializeField] private float wallOffset = 10f; // Distance to place walls beyond the furthest maps

        [Space]
        [SerializeField] private List<MapComponent> maps;
        [SerializeField] private ObjectPooler<MapComponent> mapPooler = new ObjectPooler<MapComponent>();

        // Track the furthest spawned positions to avoid duplicate spawning
        private float furthestPositiveZ = 0f;
        private float furthestNegativeZ = 0f;

        private void Awake()
        {
            foreach (MapComponent map in maps)
            {
                mapPooler.Init(map, 2, mapPool);
            }
        }

        private void Start()
        {
            //// Initial spawn - 4 maps ahead
            //for (int i = 0; i < 4; i++)
            //{
            //    float zPos = (i + 1) * mapSpacing;
            //    SpawnMap(new Vector3(0, 0, zPos));
            //    furthestPositiveZ = zPos;
            //}

            // Initial fog wall positioning
            UpdateFogWalls();
        }

        private void Update()
        {
            CheckAndManageMaps();
        }

        private void CheckAndManageMaps()
        {
            // First, check existing active maps and return those too far away
            List<Transform> mapsToRemove = new List<Transform>();

            foreach (Transform child in activeMapParent)
            {
                if (child.TryGetComponent<MapComponent>(out MapComponent map))
                {
                    float distance = Mathf.Abs(child.position.z - playerTransform.position.z);

                    if (distance > mapRenderDistance)
                    {
                        // Mark for removal (can't modify collection while iterating)
                        mapsToRemove.Add(child);
                    }
                }
            }

            // Return distant maps to pool
            foreach (Transform mapTransform in mapsToRemove)
            {
                if (mapTransform.TryGetComponent<MapComponent>(out MapComponent map))
                {
                    ReturnMap(map);
                }
            }

            // Then, spawn new maps ahead of the player if needed
            bool mapsChanged = SpawnMapsAhead();

            // Update fog walls if maps changed
            if (mapsChanged || mapsToRemove.Count > 0)
            {
                UpdateFogWalls();
            }
        }

        private bool SpawnMapsAhead()
        {
            float playerZ = playerTransform.position.z;
            bool mapsSpawned = false;

            // Check if we need to spawn maps in front of player (positive Z direction)
            float nextPositiveZ = GetNextMapPosition(playerZ, true);
            while (nextPositiveZ - playerZ < mapRenderDistance)
            {
                SpawnMap(new Vector3(0, 0, nextPositiveZ));
                furthestPositiveZ = Mathf.Max(furthestPositiveZ, nextPositiveZ);
                nextPositiveZ += mapSpacing;
                mapsSpawned = true;
            }

            // Check if we need to spawn maps behind player (negative Z direction)
            float nextNegativeZ = GetNextMapPosition(playerZ, false);
            while (playerZ - nextNegativeZ < mapRenderDistance)
            {
                SpawnMap(new Vector3(0, 0, nextNegativeZ));
                furthestNegativeZ = Mathf.Min(furthestNegativeZ, nextNegativeZ);
                nextNegativeZ -= mapSpacing;
                mapsSpawned = true;
            }

            return mapsSpawned;
        }

        // Helper method to find the next map position that needs to be spawned
        private float GetNextMapPosition(float playerZ, bool positiveDirection)
        {
            if (positiveDirection)
            {
                // Find the next positive Z position that should have a map
                float basePosition = Mathf.Ceil(playerZ / mapSpacing) * mapSpacing;

                // Check if this position already has a map
                while (HasMapAtPosition(basePosition))
                {
                    basePosition += mapSpacing;
                }
                return basePosition;
            }
            else
            {
                // Find the next negative Z position that should have a map
                float basePosition = Mathf.Floor(playerZ / mapSpacing) * mapSpacing;

                // Check if this position already has a map
                while (HasMapAtPosition(basePosition))
                {
                    basePosition -= mapSpacing;
                }
                return basePosition;
            }
        }

        // Check if there's already a map at or near this position
        private bool HasMapAtPosition(float zPosition)
        {
            foreach (Transform child in activeMapParent)
            {
                if (Mathf.Abs(child.position.z - zPosition) < mapSpacing * 0.1f) // Small tolerance
                {
                    return true;
                }
            }
            return false;
        }

        private void SpawnMap(Vector3 position)
        {
            MapComponent map = mapPooler.GetRandom();
            map.transform.SetParent(activeMapParent);
            map.transform.position = position;
            map.Init(playerTransform, this);
        }

        public void ReturnMap(MapComponent mapComponent)
        {
            mapComponent.transform.SetParent(mapPool);
            mapPooler.Return(mapComponent);
        }

        /// <summary>
        /// Updates the fog wall positions based on the current active map boundaries
        /// </summary>
        private void UpdateFogWalls()
        {
            if (minZWall == null || maxZWall == null) return;

            // Find the actual min and max Z positions of active maps
            float minZ = float.MaxValue;
            float maxZ = float.MinValue;

            foreach (Transform child in activeMapParent)
            {
                float childZ = child.position.z;
                minZ = Mathf.Min(minZ, childZ);
                maxZ = Mathf.Max(maxZ, childZ);
            }

            // If no active maps, use furthest positions
            if (minZ == float.MaxValue)
            {
                minZ = furthestNegativeZ;
                maxZ = furthestPositiveZ;
            }

            // Position the walls with offset
            Vector3 minWallPos = minZWall.position;
            minWallPos.z = minZ - wallOffset;
            minZWall.position = minWallPos;

            Vector3 maxWallPos = maxZWall.position;
            maxWallPos.z = maxZ + wallOffset - mapSpacing;
            maxZWall.position = maxWallPos;
        }

        /// <summary>
        /// Public method to manually update fog walls if needed
        /// </summary>
        public void ForceUpdateFogWalls()
        {
            UpdateFogWalls();
        }

        // Optional: Debug method to visualize the system
        private void OnDrawGizmosSelected()
        {
            if (playerTransform == null) return;

            // Draw render distance sphere
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, mapRenderDistance);

            // Draw spawn points
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(new Vector3(0, 0, furthestPositiveZ), Vector3.one * 2f);
            Gizmos.DrawWireCube(new Vector3(0, 0, furthestNegativeZ), Vector3.one * 2f);

            // Draw fog wall positions
            if (minZWall != null && maxZWall != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(minZWall.position, new Vector3(10f, 5f, 1f));
                Gizmos.DrawWireCube(maxZWall.position, new Vector3(10f, 5f, 1f));
            }
        }
    }
}