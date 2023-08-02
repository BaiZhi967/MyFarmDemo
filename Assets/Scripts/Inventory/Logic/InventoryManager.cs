using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WzFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        
        [Header("物品数据")]
        public ItemDataList_SO _ItemDataListSo;

        [Header("背包数据")] public InventoryBag_SO PlayerBag;

        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player,PlayerBag.itemList);
            
        }
        
        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
        }

    

        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
        }

        private void OnDropItemEvent(int itemID, Vector3 pos,ItemType itemType)
        {
            RemoveItem(itemID,1);
        }


        /// <summary>
        /// 通过ID查找物品
        /// </summary>
        /// <param name="ID">物品的ID</param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return _ItemDataListSo.itemDetailsList.Find(i => i.itemID == ID);
        }

        /// <summary>
        /// 添加物品到Plyaer背包
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="isDestory">是否销毁物品</param>
        public void AddItem(Item item, bool isDestory)
        {
            
            var index = GetItemIndexInBag(item.itemID);
            
            AddItemAtIndex(item.itemID,index,1);
                

            
            Debug.Log(GetItemDetails(item.itemID).itemID + "Name: " + GetItemDetails(item.itemID).itemName);
            if (isDestory)
            {
                Destroy(item.gameObject);
            }
            
            //更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player,PlayerBag.itemList);
        }

        /// <summary>
        /// 判断背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < PlayerBag.itemList.Count; i++)
            {
                if (PlayerBag.itemList[i].itemID == 0)
                    return true;
            }

            return false;
        }

        
        /// <summary>
        /// 检测物品在背包的位置
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns>返回物品下标，若为-1则表示没查找到</returns>
        private int GetItemIndexInBag(int id)
        {
            for (int i = 0; i < PlayerBag.itemList.Count; i++)
            {
                if (PlayerBag.itemList[i].itemID == id)
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// 在指定背包序号位置添加物品
        /// </summary>
        /// <param name="id">物品ID</param>
        /// <param name="index">序号</param>
        /// <param name="amount">数量</param>
        private void AddItemAtIndex(int id, int index, int amount)
        {
            if (index == -1 && CheckBagCapacity())    //背包没有这个物品 同时背包有空位
            {
                var item = new InventoryItem { itemID = id, itemAmount = amount };
                for (int i = 0; i < PlayerBag.itemList.Count; i++)
                {
                    if (PlayerBag.itemList[i].itemID == 0)
                    {
                        PlayerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else    //背包有这个物品
            {
                int currentAmount = PlayerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = id, itemAmount = currentAmount };

                PlayerBag.itemList[index] = item;
            }
        }
        
        /// <summary>
        /// Player背包范围内交换物品
        /// </summary>
        /// <param name="fromIndex">起始序号</param>
        /// <param name="targetIndex">目标数据序号</param>
        public void SwapItem(int fromIndex, int targetIndex)
        {
            InventoryItem currentItem = PlayerBag.itemList[fromIndex];
            InventoryItem targetItem = PlayerBag.itemList[targetIndex];

            if (targetItem.itemID != 0)
            {
                PlayerBag.itemList[fromIndex] = targetItem;
                PlayerBag.itemList[targetIndex] = currentItem;
            }
            else
            {
                PlayerBag.itemList[targetIndex] = currentItem;
                PlayerBag.itemList[fromIndex] = new InventoryItem();
            }
            
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, PlayerBag.itemList);
        }
        
        /// <summary>
        /// 移除指定数量的背包物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="removeAmount">数量</param>
        private void RemoveItem(int ID, int removeAmount)
        {
            var index = GetItemIndexInBag(ID);

            if (PlayerBag.itemList[index].itemAmount > removeAmount)
            {
                var amount = PlayerBag.itemList[index].itemAmount - removeAmount;
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                PlayerBag.itemList[index] = item;
            }
            else if (PlayerBag.itemList[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                PlayerBag.itemList[index] = item;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, PlayerBag.itemList);
        }

        
    }
    
}
