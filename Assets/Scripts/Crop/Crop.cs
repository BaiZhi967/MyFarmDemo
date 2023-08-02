using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;

    private int harvestActionCount;
    private TileDetails _tileDetails;
    public void ProcessToolAction(ItemDetails tool,TileDetails tile)
    {
        _tileDetails = tile;
        
        //工具使用次数
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        if(requireActionCount == -1) return;
        
        //判断是否有动画 例:树木
        
        //点击计数器
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;
            //播放声音、特效
        }

        if (harvestActionCount >= requireActionCount)
        {
            if (cropDetails.generateAtPlayerPosition)
            {
                //生成农作物
                SpwanHarvestItems();
            }
        }
        
    }

    public void SpwanHarvestItems()
    {
        int amountToProduce = 0;
        for (int i = 0; i < cropDetails.producedItemID.Length; i++)
        {
            amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
            //Debug.Log(amountToProduce);
            for (int j = 0; j < amountToProduce; j++)
            {
                if (cropDetails.generateAtPlayerPosition)
                {
                    //直接生成在玩家背包
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }
                else
                {
                    //生成在世界上
                }
            }
        }

        if (_tileDetails != null)
        {
            _tileDetails.daysSinceLastHarvest++;
            
            //是否可以重复生长
            if (cropDetails.daysToRegrow > 0 && _tileDetails.daysSinceLastHarvest < cropDetails.regrowTimes)
            {
                _tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                //刷新种子
                EventHandler.CallRefreshCurrentMap();
            }
            else
            {
                _tileDetails.daysSinceLastHarvest = -1;
                _tileDetails.seedItemID = -1;
            }
            
            Destroy(gameObject);
        }

        
    }
}
