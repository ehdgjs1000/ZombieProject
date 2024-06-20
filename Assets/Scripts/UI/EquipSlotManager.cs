using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlotManager : MonoBehaviour
{
    [SerializeField] Image helmetImg;
    [SerializeField] Image armorImg;
    [SerializeField] Image bagImg;

    [SerializeField] Sprite[] helmetSprites;
    [SerializeField] Sprite[] armorSprites;
    [SerializeField] Sprite[] bagSprites;

    public static int helmetLv=0;
    public static int armorLv=0;
    public static int bagLv=1;

    private void Update()
    {
        CheckEquipLevel();
    }
    public void CheckEquipLevel()
    {
        switch (helmetLv)
        {
            case 0:
                helmetImg.sprite = null;
                break;
            case 1:
                helmetImg.sprite = helmetSprites[0];
                break;
            case 2:
                helmetImg.sprite = helmetSprites[1];
                break;
            case 3:
                helmetImg.sprite = helmetSprites[2];
                break;
        }
        switch (armorLv)
        {
            case 0:
                armorImg.sprite = null;
                break;
            case 1:
                armorImg.sprite = armorSprites[0];
                break;
            case 2:
                armorImg.sprite = armorSprites[1];
                break;
            case 3:
                armorImg.sprite = armorSprites[2];
                break;
        }
        switch (bagLv)
        {
            case 0:
                bagImg.sprite = null;
                break;
            case 1:
                bagImg.sprite = bagSprites[0];  
                break;
            case 2:
                bagImg.sprite = bagSprites[1];  
                break;
            case 3:
                bagImg.sprite = bagSprites[2];
                break;
        }

        if(helmetLv == 0)
        {
            helmetImg.enabled = false;    
        }else
        {
            helmetImg.enabled = true;
        }
        if (armorLv == 0)
        {
            armorImg.enabled = false;
        }
        else
        {
            armorImg.enabled = true;
        }
        if (bagLv == 0)
        {
            bagImg.enabled = false;
        }
        else
        {
            bagImg.enabled = true;
        }

    }

}
