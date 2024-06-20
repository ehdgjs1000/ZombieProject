using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] public float damage;

    [SerializeField] ThirdPersonShooterController TPSC;


    private void OnTriggerEnter(Collider hit)
    {
        if (hit.transform.tag == "DeafZombie")
        {
            TPSC.PublicPlaySFX(2);
            hit.transform.gameObject.GetComponent<DeafZombieController>().zombieHp -= damage;
        }
        else if (hit.transform.tag == "CrawlZombie")
        {
            TPSC.PublicPlaySFX(2);
            hit.transform.gameObject.GetComponent<CrawlZombieController>().zombieHp -= damage;
        }
        else if (hit.transform.tag == "ShortZombie")
        {

            TPSC.PublicPlaySFX(2);
            hit.transform.gameObject.GetComponent<ShortZombieController>().zombieHp -= damage;
        }
        else if (hit.transform.tag == "LongZombie")
        {
            TPSC.PublicPlaySFX(2);
            hit.transform.gameObject.GetComponent<LongZombieController>().zombieHp -= damage;
        }
    }
}
