using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotToolTip : MonoBehaviour
{
    [SerializeField] GameObject go_Base;
    [SerializeField] Text itemNameTxt;
    [SerializeField] Text weaponDamageTxt;
    [SerializeField] Text weaponRateTxt;
    [SerializeField] Text howToUse;

    [SerializeField] Text healAmountTxt;
    [SerializeField] Text thirstAmountTxt;
    [SerializeField] Text hungryAmountTxt;

    [SerializeField] GameObject weaponTxtSet;
    [SerializeField] GameObject healTxtSet;


    public void ShowToolTop(Item _item)
    {
        go_Base.SetActive(true);

        go_Base.transform.position = new Vector2(Input.mousePosition.x + 300f, Input.mousePosition.y - 200f);

        itemNameTxt.text = _item.itemName;
        if (_item.itemType == Item.ItemType.Weapon)
        {
            weaponTxtSet.SetActive(true);
            healTxtSet.SetActive(false);
            weaponDamageTxt.text = _item.damage.ToString();
            weaponRateTxt.text = _item.fireRate.ToString();
            howToUse.text = "Drag - ÀåÂø";
        }
        else if (_item.itemType == Item.ItemType.Used)
        {
            weaponTxtSet.SetActive(false);
            healTxtSet.SetActive(true);
            healAmountTxt.text = _item.healAmount.ToString();
            thirstAmountTxt.text = _item.thristAmonut.ToString();
            hungryAmountTxt.text = _item.hungryAmount.ToString();
            howToUse.text = "Right Click - ¼Ò¸ð";
        }
        else if(_item.itemType == Item.ItemType.Equipment)
        {
            weaponTxtSet.SetActive(false);
            healTxtSet.SetActive(false);
            howToUse.text = "Right Click - ÀåÂø";
        }
        else
        {
            weaponTxtSet.SetActive(false);
            healTxtSet.SetActive(false);
            howToUse.text = "";
        }

    }
    public void HideToolTop()
    {
        go_Base.SetActive(false);
    }

}
