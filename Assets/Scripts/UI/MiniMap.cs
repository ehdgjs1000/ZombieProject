using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{

    [SerializeField] float miniMapOffset = 10.0f;
    [SerializeField] Transform playerTr;
    private void LateUpdate()
    {
        transform.SetPositionAndRotation(new Vector3(playerTr.transform.position.x,
            playerTr.transform.position.y + miniMapOffset, playerTr.transform.position.z),
            Quaternion.Euler(90f, playerTr.eulerAngles.y, 0f));

    }
}
