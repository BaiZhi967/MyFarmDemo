using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WzFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("背包状态")] [SerializeField] private GameObject bagUI;
        private bool bagOpenned;
        
        [SerializeField] private SlotUI[] playerSlots;

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
        }
        /// <summary>
        /// 开关背包
        /// </summary>
        /// <returns>背包开关状态</returns>
        public void OpenBagUI()
        {
            bagOpenned = !bagOpenned;
            bagUI.SetActive(bagOpenned);
            //return bagOpenned;
        }

        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item,list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
        }

        private void Start()
        {
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            bagOpenned = bagUI.activeInHierarchy;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OpenBagUI();
            }
        }
        
        /// <summary>
        /// 更新Slot高亮显示
        /// </summary>
        /// <param name="index">序号</param>
        public void UpdateSlotHightlight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if (slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHighLight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHighLight.gameObject.SetActive(false);
                }
            }
        }
        
    }
}


