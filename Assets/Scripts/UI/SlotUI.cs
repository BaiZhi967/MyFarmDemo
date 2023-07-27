using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace WzFarm.Inventory
{
    public class SlotUI : MonoBehaviour
    {
        [Header("组件获取")] 
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amontText;
        [SerializeField] private Image slotHighLight;
        [SerializeField] private Button _button;
        [Header("盒子类型")] public SlotType slotType;
    
        public bool isSelected;
        public ItemDetails _ItemDetails;
        public int itemAmount;
        public int slotIndex;
    
        private void Start()
        {
            isSelected = false;
            if (_ItemDetails.itemID == 0)
            {
                UpdateEmptySlot();
            }
        }
    
        /// <summary>
        /// 更新UI和数据
        /// </summary>
        /// <param name="item">物品信息</param>
        /// <param name="amount">物品数量</param>
        public void UpdateSlot(ItemDetails item,int amount)
        {
            _ItemDetails = item;
            slotImage.sprite = item.itemIcon;
            itemAmount = amount;
            amontText.text = amount.ToString();
            _button.interactable = true;
            slotImage.enabled = true;
        }
        
        
        
        /// <summary>
        /// 将slot更新为空
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected) isSelected = !isSelected;
            slotImage.enabled = false;
            amontText.text = String.Empty;
            _button.interactable = false;
        }
    }

}