using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WzFarm.CropPlant
{
    public class CropManager : MonoBehaviour
    {
        public CropDetaList_SO cropData;
        private Transform cropParent;
        private Grid currentGrid;
        private Season currentSeason;

        private void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += GameDayEvent;
        }

        private void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= GameDayEvent;
        }

        private void GameDayEvent(int day, Season season)
        {
            currentSeason = season;
        }


        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            cropParent = GameObject.FindWithTag("CropParent").transform;
        }

        private void OnPlantSeedEvent(int id, TileDetails tileDetails)
        {
            CropDetails currentCrop = GetCropDetails(id);
            if (currentCrop != null && SeasonAvailable(currentCrop)&& tileDetails.seedItemID == -1)//用于第一次
            {
                tileDetails.seedItemID = id;
                tileDetails.growthDays = 0;
                //显示农作物
                DisplayCropPlant(tileDetails, currentCrop);

            }else if (tileDetails.seedItemID != -1)//用于刷新地图
            {
                DisplayCropPlant(tileDetails, currentCrop);
            }
        }
        
        
        /// <summary>
        /// 显示农作物
        /// </summary>
        /// <param name="tileDetails">瓦片地图信息</param>
        /// <param name="cropDetails">种子信息</param>
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            //成长阶段
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;

            //倒序计算当前的成长阶段
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }

            //获取当前阶段的Prefab
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];

            Vector3 pos = new Vector3(tileDetails.girdX + 0.5f, tileDetails.gridY + 0.5f, 0);

            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;
        }
        
        
        /// <summary>
        /// 通过id查找种子信息
        /// </summary>
        /// <param name="id">种子id</param>
        /// <returns>种子信息</returns>
        private CropDetails GetCropDetails(int id)
        {
            return cropData.CropDetailsList.Find(c=>c.seedItemID == id);
        }

        /// <summary>
        /// 判断当前季节是否可以种植
        /// </summary>
        /// <param name="cropDetails">种子信息</param>
        /// <returns>可否种植</returns>
        private bool SeasonAvailable(CropDetails cropDetails)
        {
            for (int i = 0; i < cropDetails.seasons.Length; i++)
            {
                if (cropDetails.seasons[i].Equals(currentSeason))
                {
                    return true;
                }
            }

            return false;
        }
    }

}