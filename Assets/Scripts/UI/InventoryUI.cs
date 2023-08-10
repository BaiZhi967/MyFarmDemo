using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace WzFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("拖拽图片")] public Image drapImage;
        [Header("物品详情页")] public ItemTooltip _itemTooltip;
        [Header("背包状态")] [SerializeField] private GameObject bagUI;
        private bool bagOpenned;
        [Header("通用背包")] [SerializeField] private GameObject baseBag;
        public GameObject slotShopPrefab;
        
        
        [SerializeField] private SlotUI[] playerSlots;
        [SerializeField] private List<SlotUI> baseBagSlots;

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
        }

        

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
        }

        private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagSo)
        {
            baseBag.SetActive(false);
            _itemTooltip.gameObject.SetActive(false);
            UpdateSlotHightlight(-1);
            foreach (var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();
        }

        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagSo)
        {
            GameObject prefab = slotType switch
            {
                SlotType.Shop => slotShopPrefab,
                _ => null,
            };
            
            baseBag.SetActive(true);

            baseBagSlots = new List<SlotUI>();
            for (int i = 0; i < bagSo.itemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());
            OnUpdateInventoryUI(InventoryLocation.Box,bagSo.itemList);
        }

        
        private void OnBeforeSceneUnloadEvent()
        {
            UpdateSlotHightlight(-1);
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
                case InventoryLocation.Box:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            baseBagSlots[i].UpdateSlot(item,list[i].itemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].UpdateEmptySlot();
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


