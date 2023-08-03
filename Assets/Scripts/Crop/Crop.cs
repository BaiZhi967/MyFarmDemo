using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;

    private int harvestActionCount;
    public TileDetails _tileDetails;
    private Animator _animator;
    private Transform PlayerTransform => FindObjectOfType<Player>().transform;
    public bool CanHavrest => _tileDetails.growthDays >= cropDetails.TotalGrowthDays;
    public void ProcessToolAction(ItemDetails tool,TileDetails tile)
    {
        _tileDetails = tile;
        
        //工具使用次数
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        if(requireActionCount == -1) return;
        _animator = GetComponentInChildren<Animator>();
        
        //Debug.Log(111);
        //点击计数器
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;
            //判断是否有动画 例:树木
            if (_animator != null && cropDetails.hasAnimation)
            {
                //Debug.Log(222);
                if (PlayerTransform.position.x < transform.position.x)
                {
                    _animator.SetTrigger("RotateRight");
                }
                else
                {
                    _animator.SetTrigger("RotateLeft");
                }
            }
            
            //播放特效
            if (cropDetails.hasParticleEffect)
            {
                EventHandler.CallParticleEffectEvent(cropDetails.effectType,transform.position+cropDetails.effectPos);
            }
            //播放声音
        }

        if (harvestActionCount >= requireActionCount)
        {
            if (cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation)
            {
                //生成农作物
                SpwanHarvestItems();
            }else if (cropDetails.hasAnimation)
            {
                if (PlayerTransform.position.x < transform.position.x)
                {
                    _animator.SetTrigger("FallingRight");
                }
                else
                {
                    _animator.SetTrigger("FallingLeft");
                }

                StartCoroutine(HarvestAfterAnimation());
            }
        }
        
    }

    private IEnumerator HarvestAfterAnimation()
    {
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("END") is false)
        {
            yield return null;
        }
        SpwanHarvestItems();
        if (cropDetails.transferItemID > 0)
        {
            CreateTransferCrop();
        }
    }


    private void CreateTransferCrop()
    {
        _tileDetails.seedItemID = cropDetails.transferItemID;
        _tileDetails.daysSinceLastHarvest = -1;
        _tileDetails.growthDays = 0;
        EventHandler.CallRefreshCurrentMap();
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
                    var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                    var spwanPos =
                        new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                            transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y),0);
                    
                    EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i],spwanPos);
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
