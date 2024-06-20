using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpawnPanel : MonoBehaviour
{
    [SerializeField] GameObject playerTr;
    [SerializeField] Button[] baseButton;

    public float clampX;
    public float clampY;
    public float clampZ;

    private void Update()
    {
        for (int a = 0; a < CharacterManager.baseCount; a++)
        {
            Vector3 basePos = CharacterManager.basePos[a];
            baseButton[a].gameObject.SetActive(true);
            baseButton[a].transform.localPosition = new Vector3(basePos.x - clampX, basePos.z - clampZ,
                basePos.y - clampY);
        }
    }

}
