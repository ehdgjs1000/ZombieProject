using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZoneController : MonoBehaviour
{
    public bool isPlayerDie = false;
    public bool isBossDie = false;
    [SerializeField] private GameObject bossGO;
    [SerializeField] private GameObject[] zombiesGO;
    [SerializeField] private Vector3[] zombiesStartPos;

    /*private void Awake()
    {

        for (int a = 0; a < zombiesGO.Length; a++)
        {
            if (zombiesGO != null)
            {
                zombiesStartPos[a] = zombiesGO[a].transform.position;
            }
        }
    }*/
    private void Update()
    {
        CheckPlayerIn();
    }
    private void CheckPlayerIn()
    {
        

    }


    private void BossDie()
    {
        isBossDie = true;
    }
    private void BossReset()
    {
        BossZombieController bc = bossGO.GetComponent<BossZombieController>();
        for (int a = 0; a < zombiesGO.Length; a++)
        {
            if (zombiesGO != null)
            {
                zombiesGO[a].transform.position = zombiesStartPos[a];
            }
        }

        bc.BossReset();

    }
}
