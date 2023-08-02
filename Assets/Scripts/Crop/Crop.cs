using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;

    public int harvestActionCount;
    public void ProcessToolAction(ItemDetails tool)
    {
        
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
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }
            }
        }

        
    }
}
