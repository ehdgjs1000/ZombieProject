using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBossZone : MonoBehaviour
{
    public static bool inBossZone = false;
    public float radius = 0.5f;

    public Collider[] collider;
    public LayerMask bossZoneLayer;

    [SerializeField] GameObject playerGo;

    private void Update()
    {
        collider = Physics.OverlapSphere(playerGo.transform.position, radius,bossZoneLayer);

        if(collider.Length != 0) inBossZone = true;
        else inBossZone = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(playerGo.transform.position, radius);
    }

}
