using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WzFarm.Inventory
{
    public class ItemPickUP : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                if (item._itemDetails.canPickedup)
                {
                    InventoryManager.Instance.AddItem(item,true);
                }
            }
        }
    }

}
