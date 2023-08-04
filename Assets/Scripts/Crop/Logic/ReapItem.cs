using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WzFarm.CropPlant
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;

        public void InitCropDetails(int id)
        {
            cropDetails = CropManager.Instance.GetCropDetails(id);
        }
        
        /// <summary>
            /// 生成果实
            /// </summary>
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
            }
    }

}