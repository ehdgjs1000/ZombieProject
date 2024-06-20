using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideCtrl : MonoBehaviour
{
    private void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.tag == "Player")
        {
            CharacterManager.instance.characterHp = 0.0f;
        }
    }
}
