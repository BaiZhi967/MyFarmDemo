using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WzFarm.Inventory
{
    public class Item : MonoBehaviour
    {
        public int itemID;
        private SpriteRenderer _spriteRenderer;
        public ItemDetails _itemDetails;
        private BoxCollider2D _collider2D;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _collider2D = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (itemID != 0)
            {
                Init(itemID);
            }
        }

        public void Init(int ID)
        {
            itemID = ID;
            _itemDetails = InventoryManager.Instance.GetItemDetails(itemID);
            if (_itemDetails != null)
            {
                _spriteRenderer.sprite = _itemDetails.itemOnWorldSprite != null
                    ? _itemDetails.itemOnWorldSprite
                    : _itemDetails.itemIcon;
                
                //修改碰撞体尺寸
                Vector2 newSize = new Vector2(_spriteRenderer.sprite.bounds.size.x, _spriteRenderer.sprite.bounds.size.y);
                _collider2D.size = newSize;
                _collider2D.offset = new Vector2(0, _spriteRenderer.sprite.bounds.center.y);
            }
        }
    }

}