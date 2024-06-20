using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler, IDropHandler
{
    private Vector3 originPos;
    public Item item;
    public int itemCount;
    public Image itemImg;
    public Image coolImg;

    [SerializeField] Text textCount;

    [SerializeField] SlotToolTip theSlotToolTop;

    private void Start()
    {
        originPos = transform.position;
    }
    private void Update()
    {
        if (itemCount == 0)
        {
            ClearSlot();
        }
        if (item != null && item.itemName == "물통")
        {
            coolImg.gameObject.SetActive(true);
            coolImg.fillAmount = item.purifyTime / 10.0f;
            item.purifyTime -= Time.deltaTime;
        }
        else
        {
            coolImg.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        ClearSlot();
    }

    //Sprite A값 변경
    private void SetColor(float _a)
    {
        Color color = itemImg.color;
        color.a = _a;
        itemImg.color = color;

    }

    //아이템 획득
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImg.sprite = item.itemImg;

        if (item.itemType != Item.ItemType.Equipment)
        {
            //오류나면 Gameobject로 만들어서 Setactive 관리하기!!
            textCount.enabled = true;
            textCount.text = itemCount.ToString();
        }
        else
        {
            textCount.text = "0";
            textCount.enabled = false;
        }


        SetColor(1);
    }

    //아이템 갯수
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        //itemImg.sprite = item.itemImg;
        textCount.text = itemCount.ToString();

        if (itemCount <= 0)
        {
            ClearSlot();
        }

    }

    //칸 비우기
    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImg.sprite = null;
        SetColor(0);

        textCount.text = "0";
        textCount.enabled = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                //장비 장착
                if (item.itemType == Item.ItemType.Equipment)
                {
                    if (item.equipType == "Helmet")
                    {
                        EquipSlotManager.helmetLv = item.equipLv;
                        SetSlotCount(-1);
                    }
                    else if (item.equipType == "Armor")
                    {
                        EquipSlotManager.armorLv = item.equipLv;
                        SetSlotCount(-1);
                    }
                    else if (item.equipType == "Bag")
                    {
                        EquipSlotManager.bagLv = item.equipLv;
                        SetSlotCount(-1);
                    }

                }
                else if (item.itemType == Item.ItemType.Used)
                {
                    if (item.itemName == "해독제")
                    {
                        for (int i = 2; i < CharacterManager.instance.debuffType.Length; i++)
                        {
                            CharacterManager.instance.debuffType[i] = false;
                        }
                        SetSlotCount(-1);
                    }
                    else if (item.itemName == "썩은 고기")
                    {
                        StartCoroutine(ActionCtrl.instance.PlaySfx(1));
                        CharacterManager.instance.IncreasetWater(item.thristAmonut);
                        CharacterManager.instance.IncreaseHungry(item.hungryAmount);
                        SetSlotCount(-1);
                        int ranNum = Random.Range(0,10);
                        if (ranNum < 2) CharacterManager.instance.debuffType[4] = true;
                    }
                    else if (item.itemName == "붕대")
                    {
                        CharacterManager.instance.IncreaseHp(item.healAmount);
                        CharacterManager.instance.debuffType[1] = true;
                        SetSlotCount(-1);
                    }
                    else
                    {
                        StartCoroutine(ActionCtrl.instance.PlaySfx(0));
                        CharacterManager.instance.IncreaseHp(item.healAmount);
                        CharacterManager.instance.IncreasetWater(item.thristAmonut);
                        CharacterManager.instance.IncreaseHungry(item.hungryAmount);
                        SetSlotCount(-1);
                    }
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImg);
            DragSlot.instance.transform.position = eventData.position;
        }

    }
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();
        }
    }
    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if (_tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        else
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            theSlotToolTop.ShowToolTop(item);
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        theSlotToolTop.HideToolTop();
    }
}