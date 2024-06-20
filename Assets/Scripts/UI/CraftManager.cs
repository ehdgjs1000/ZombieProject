using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftManager : MonoBehaviour
{
    [SerializeField] GameObject[] slotSets;

    public void TabBtnOnClick(int _tabNum)
    {
        for (int i = 0; i < slotSets.Length; i++)
        {
            if(i == _tabNum)
            {
                slotSets[i].SetActive(true);
            }else slotSets[i].SetActive(false);
        }

    }

}
