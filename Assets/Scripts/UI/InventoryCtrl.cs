using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCtrl : MonoBehaviour
{
    [SerializeField] private GameObject go_SlotsParent;

    public static InventoryCtrl instance;
    //슬롯들
    public Inventory[] slots;

    //핸드 슬록
    public Inventory[] handSlots;
    public Image[] handImg;
    [SerializeField] Inventory[] bagSlots;


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Inventory>();
    }
    void Update()
    {
        SetBagSlots();
        for (int i = 0; i < 4; i++)
        {
            if(handSlots[i].item == null)
            {
                SetColor(handImg[i], 0);
            }
            else
            {
                handImg[i].sprite = handSlots[i].item.itemImg;
                SetColor(handImg[i], 1);

            }
            
        }
    }
    private void SetBagSlots()
    {
        int bagLv = EquipSlotManager.bagLv;
        if(bagLv == 2)
        {
            for (int i = 0; i < 5; i++)
            {
                bagSlots[i].gameObject.SetActive(true);
            }
        }else if(bagLv == 3)
        {
            for (int i = 0; i < 10; i++)
            {
                bagSlots[i].gameObject.SetActive(true);
            }
        }
    }
    private void SetColor(Image img, float _a)
    {
        Color color = img.color;
        color.a = _a;
        img.color = color;
    }
    public void ReloadAmmo(int ammoLv, int _count = 0)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.ammoLevel == ammoLv)
                {
                    slots[i].SetSlotCount(-_count);
                    return;
                }
            }
        }
    }
    public void BuildObject(Item _item, int _count)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].item == _item)
            {
                slots[i].SetSlotCount(_count);
                return;
            }
        }
    }

    public void AcquireItem(Item _item, int _count = 1)
    {
        if(Item.ItemType.Equipment != _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                //최대 용량 추후에 추가하기
                if (slots[i].item != null)
                {
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }else if (slots[i].item == null)
                {
                    slots[i].SetSlotCount(0);
                }
                
            }
        }
        //장비 아이템
        if(Item.ItemType.Equipment == _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                //최대 용량 추후에 추가하기
                if (slots[i].item != null)
                {
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
                else if (slots[i].item == null)
                {
                    slots[i].SetSlotCount(0);
                }

            }
        }

        //아이템이 장비일 경우는 갯수 ㄴㄴ
        for (int i = 0; i <slots.Length; i++)
        {
            if(slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }

        }


    }

}
