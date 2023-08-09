using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WzFarm.Map;

namespace WzFarm.CropPlant
{
    

    public class CropGenerator : MonoBehaviour
    {
        private Grid currentGrid;
        public int seedItemId;
        public int growthDays;

        private void Awake()
        {
            currentGrid = FindObjectOfType<Grid>();
        }
        private void OnEnable()
        {
            EventHandler.GenerateCropEvent += GenerateCrop;
        }

        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= GenerateCrop;
        }

        private void GenerateCrop()
        {
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);
            if (seedItemId > 0)
            {
                var tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);
                if (tile is null)
                {
                    tile = new TileDetails();
                    tile.girdX = cropGridPos.x;
                    tile.gridY = cropGridPos.y;
                }

                tile.daysSinceWatered = -1;
                tile.seedItemID = seedItemId;
                tile.growthDays = growthDays;
                GridMapManager.Instance.UpdateTileDetails(tile);
            }
        }
    }

}