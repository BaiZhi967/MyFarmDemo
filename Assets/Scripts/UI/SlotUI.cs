using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace WzFarm.Inventory
{
    public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IEndDragHandler,IDragHandler
    {
        [Header("组件获取")] 
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amontText;
        [SerializeField] public Image slotHighLight;
        [SerializeField] private Button _button;
        [Header("盒子类型")] public SlotType slotType;
    
        public bool isSelected;
        public ItemDetails _ItemDetails;
        public int itemAmount;
        public int slotIndex;

        private InventoryUI _inventoryUI => GetComponentInParent<InventoryUI>();
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

        public void OnPointerClick(PointerEventData eventData)
        {
            //点击高亮
            isSelected = !isSelected;
            _inventoryUI.UpdateSlotHightlight(this.slotIndex);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (this.itemAmount > 0)
            {
                _inventoryUI.drapImage.enabled = true;
                _inventoryUI.drapImage.sprite = this.slotImage.sprite;
                _inventoryUI.drapImage.SetNativeSize();
                isSelected = true;
                _inventoryUI.UpdateSlotHightlight(this.slotIndex);
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            _inventoryUI.drapImage.transform.position = Input.mousePosition;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            _inventoryUI.drapImage.enabled = false;
            Debug.Log(eventData.pointerCurrentRaycast.gameObject);

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                {
                    return;
                }
                
                //交换物品
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;
                if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(this.slotIndex,targetIndex);
                    targetSlot.isSelected = true;
                    _inventoryUI.UpdateSlotHightlight(targetIndex);
                }


            }
            
        }

        
    }

}