using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using WzFarm.CropPlant;

namespace WzFarm.Map
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        

        [Header("种地瓦片信息")] 
        public RuleTile digTile;
        public RuleTile waterTile;

        private Tilemap digTilemap;
        private Tilemap waterTilemap;
        
        [Header("地图信息")]
        public List<MapData_SO> mapDataList;

        //场景名字+坐标和对应的瓦片信息
        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();
        //记录场景是否第一次被加载
        private Dictionary<string, bool> firstLoadDict = new Dictionary<string, bool>();

        private Grid currentGrid;
        private Season currentSeason;

        private List<ReapItem> itemInRadius;

        private void Start()
        {
            foreach (var mapData in mapDataList)
            {
                firstLoadDict.Add(mapData.sceneName,true);
                InitTileDetailsDict(mapData);
            }
        }

        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrentMap += RefreshMap;
        }

        


        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= RefreshMap;
        }
        
        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            digTilemap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            waterTilemap =GameObject.FindWithTag("Water").GetComponent<Tilemap>();
            
            //DisplayMap(SceneManager.GetActiveScene().name);

            if (firstLoadDict[SceneManager.GetActiveScene().name])
            {
                //预先生成农作物
                EventHandler.CallGenerateCropEvent();
                firstLoadDict[SceneManager.GetActiveScene().name] = false;
            }
            RefreshMap();
        }

        /// <summary>
        /// 根据地图信息生成字典
        /// </summary>
        /// <param name="mapData">地图信息</param>
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    girdX = tileProperty.tileCoordinate.x,
                    gridY = tileProperty.tileCoordinate.y
                };

                //字典的Key
                string key = tileDetails.girdX + "x" + tileDetails.gridY + "y" + mapData.sceneName;
                //Debug.Log(key);

                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (tileProperty.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                }

                if (GetTileDetails(key) != null)
                    tileDetailsDict[key] = tileDetails;
                else
                    tileDetailsDict.Add(key, tileDetails);
            }
        }


        /// <summary>
        /// 根据key返回瓦片信息
        /// </summary>
        /// <param name="key">x+y+地图名字</param>
        /// <returns></returns>
        public TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.ContainsKey(key))
            {
                return tileDetailsDict[key];
            }
            return null;
        }


        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y + "y" + SceneManager.GetActiveScene().name;
            //Debug.Log(key);
            return GetTileDetails(key);
        }
        
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);
           
            if (currentTile != null)
            {
                Crop currentCrop =  GetCropObject(mouseWorldPos);
                //WORKFLOW:物品使用实际功能
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(itemDetails.itemID,currentTile);
                        EventHandler.CallDropItemEvent(itemDetails.itemID,mouseWorldPos,itemDetails.itemType);
                        break;
                    case ItemType.Commodity:
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos,itemDetails.itemType);
                        break;
                    case ItemType.HoeTool:
                        SetDigGround(currentTile);
                        currentTile.daysSinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        //音效
                        break;
                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daysSinceWatered = 0;
                        //音效
                        break;
                    case ItemType.CollectTool:
                        currentCrop?.ProcessToolAction(itemDetails,currentTile);
                        break;
                    case ItemType.BreakTool:
                    case ItemType.ChopTool:
                        currentCrop?.ProcessToolAction(itemDetails,currentCrop._tileDetails);
                        break;
                    case ItemType.ReapTool:
                        for (int i = 0; i < itemInRadius.Count; i++)
                        {
                            EventHandler.CallParticleEffectEvent(ParticleEffectType.ReapableScenery,itemInRadius[i].transform.position+Vector3.up);
                            itemInRadius[i].SpwanHarvestItems();
                            Destroy(itemInRadius[i].gameObject );
                        }
                        break;
                    case ItemType.Furniture:
                        EventHandler.CallBuildFurnitureEvent(itemDetails.itemID,mouseWorldPos);
                        break;
                        
                }

                UpdateTileDetails(currentTile);
            }

        }
        
        /// <summary>
        /// 通过物理方法判断鼠标点击位置的农作物
        /// </summary>
        /// <param name="mouseWorldPos">鼠标坐标</param>
        /// <returns></returns>
        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);

            Crop currentCrop = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                    currentCrop = colliders[i].GetComponent<Crop>();
            }
            return currentCrop;
        }

        /// <summary>
        /// 返回工具范围内的杂草
        /// </summary>
        /// <param name="itemDetails">工具信息</param>
        /// <returns></returns>
        public bool HavaReapableItemInRadius(Vector3 mouseWorldPos,ItemDetails itemDetails)
        {
            itemInRadius = new List<ReapItem>();
            Collider2D[] collider2Ds = new Collider2D[20];

            Physics2D.OverlapCircleNonAlloc(mouseWorldPos, itemDetails.itemUseRadius, collider2Ds);
            if (collider2Ds.Length > 0)
            {
                for (int i = 0; i < collider2Ds.Length; i++)
                {
                    if (collider2Ds[i] is not null)
                    {
                        if (collider2Ds[i].GetComponent<ReapItem>() is not null)
                        {
                            itemInRadius.Add(collider2Ds[i].GetComponent<ReapItem>());
                        }
                    }
                    
                }
            }

            return itemInRadius.Count > 0;

        }
        
        
        /// <summary>
        /// 每天执行1次
        /// </summary>
        /// <param name="day"></param>
        /// <param name="season"></param>
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;

            foreach (var tile in tileDetailsDict)
            {
                if (tile.Value.daysSinceWatered > -1)
                {
                    tile.Value.daysSinceWatered = -1;
                }
                if (tile.Value.daysSinceDug > -1)
                {
                    tile.Value.daysSinceDug++;
                }
                //超期消除挖坑
                if (tile.Value.daysSinceDug > 5 && tile.Value.seedItemID == -1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;
                }

                if (tile.Value.seedItemID != -1)
                {
                    tile.Value.growthDays++;
                }
            }

            RefreshMap();
        }

        /// <summary>
        /// 显示挖坑图片
        /// </summary>
        /// <param name="tileDetails"></param>
        private void SetDigGround(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.girdX, tileDetails.gridY, 0);
            if (digTilemap != null)
            {
                digTilemap.SetTile(pos,digTile);
            }
        }
        /// <summary>
        /// 显示浇水图片
        /// </summary>
        /// <param name="tileDetails"></param>
        private void SetWaterGround(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.girdX, tileDetails.gridY, 0);
            if (waterTilemap != null)
            {
                waterTilemap.SetTile(pos,waterTile);
            }
        }
        
        /// <summary>
        /// 更新瓦片信息
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.girdX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key,tileDetails);
            }
        }
        /// <summary>
        /// 刷新当前地图
        /// </summary>
        private void RefreshMap()
        {
            if (digTilemap != null)
                digTilemap.ClearAllTiles();
            if (waterTilemap != null)
                waterTilemap.ClearAllTiles();

            foreach (var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }

            DisplayMap(SceneManager.GetActiveScene().name);
        }


        /// <summary>
        /// 显示地图瓦片
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        private void DisplayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;

                if (key.Contains(sceneName))
                {
                    if (tileDetails.daysSinceDug > -1)
                        SetDigGround(tileDetails);
                    if (tileDetails.daysSinceWatered > -1)
                        SetWaterGround(tileDetails);
                    if (tileDetails.seedItemID != -1)
                    {
                       EventHandler.CallPlantSeedEvent(tileDetails.seedItemID,tileDetails);
                    }
                }
            }
        }
        
        /// <summary>
        /// 根据场景名称构建网格范围，绘制范围和原点
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="gridDimensions">网格范围</param>
        /// <param name="gridOrigin">网格原点</param>
        /// <returns>是否有当前场景的信息</returns>
        public bool GetGridDimensions(string sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin){
            gridDimensions = Vector2Int.zero;
            gridOrigin = Vector2Int.zero;

            foreach (var mapDataSo in mapDataList)
            {
                if (mapDataSo.sceneName==sceneName)
                {
                    gridDimensions.x = mapDataSo.gridWidth;
                    gridDimensions.y = mapDataSo.gridHeight;

                    gridOrigin.x = mapDataSo.originX;
                    gridOrigin.y = mapDataSo.originY;

                    return true;
                }
            }

            return false;
        }
    }
}