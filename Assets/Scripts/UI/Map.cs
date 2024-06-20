using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject playerTr;
    [SerializeField] Image playerPosImg;
    [SerializeField] Image[] baseImg;

    public float clampX;
    public float clampY;
    public float clampZ;

    private void Update()
    {
        playerPosImg.transform.localPosition = new Vector3(playerTr.transform.position.x - clampX,
            playerTr.transform.position.z - clampZ, playerTr.transform.position.y - clampY);

        for(int a = 0; a<CharacterManager.baseCount; a++)
        {
            Vector3 basePos = CharacterManager.basePos[a];
            baseImg[a].gameObject.SetActive(true);
            baseImg[a].transform.localPosition = new Vector3(basePos.x - clampX, basePos.z - clampZ,
                basePos.y - clampY);
        }
        
    }

}
