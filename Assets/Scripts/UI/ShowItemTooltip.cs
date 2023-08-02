using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WzFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ShowItemTooltip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        private SlotUI slotUI;
        private InventoryUI _inventoryUI => GetComponentInParent<InventoryUI>();

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI._ItemDetails != null)
            {
                _inventoryUI._itemTooltip.gameObject.SetActive(true);
                _inventoryUI._itemTooltip.SetupTooltip(slotUI._ItemDetails,slotUI.slotType);

                _inventoryUI._itemTooltip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                _inventoryUI._itemTooltip.transform.position = transform.position + Vector3.up * 60;
            }
            else
            {
                _inventoryUI._itemTooltip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _inventoryUI._itemTooltip.gameObject.SetActive(false);
        }
    }
    
    

}
