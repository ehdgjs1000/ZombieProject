using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyInformationCtrl : MonoBehaviour
{
    [SerializeField] Text[] keyInformations;

    private void Update()
    {
        if (SettingCtrl.isSettingPanelActive)
        {
            keyInformations[0].gameObject.SetActive(false);
            keyInformations[1].gameObject.SetActive(false);
            keyInformations[2].gameObject.SetActive(false);
        }
        else
        {
            if (ActionCtrl.isEquipedAxe || ActionCtrl.isEquipedShortWeapon)
            {
                keyInformations[0].gameObject.SetActive(false);
                keyInformations[1].gameObject.SetActive(true);
                keyInformations[2].gameObject.SetActive(false);
            }
            else if (ActionCtrl.isEquipedPistol || ActionCtrl.isEquipedWeapon)
            {
                keyInformations[0].gameObject.SetActive(false);
                keyInformations[1].gameObject.SetActive(false);
                keyInformations[2].gameObject.SetActive(true);
            }
            else
            {
                keyInformations[0].gameObject.SetActive(true);
                keyInformations[1].gameObject.SetActive(false);
                keyInformations[2].gameObject.SetActive(false);
            }
        }
    }



}
