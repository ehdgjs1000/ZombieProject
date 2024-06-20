using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;

    public int damage = 20;
    public float speed = 70.0f;

    private Rigidbody bulletRigid;


    private void Awake()
    {
        bulletRigid = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        bulletRigid.velocity = transform.forward * speed;

        Destroy(gameObject, 4.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("ShortWeaponArea"))
        {
            Instantiate(vfxHit, transform.position, Quaternion.identity);

            if (other.gameObject.CompareTag("ShortZombie"))
            {
                other.GetComponent<ShortZombieController>().zombieHp -= CharacterManager.weaponDmg;
            }
            else if (other.gameObject.CompareTag("LongZombie"))
            {
                other.GetComponent<LongZombieController>().zombieHp -= CharacterManager.weaponDmg;
            }
            else if (other.gameObject.CompareTag("CrawlZombie"))
            {
                other.GetComponent<CrawlZombieController>().zombieHp -= CharacterManager.weaponDmg;
            }
            else if (other.gameObject.CompareTag("DeafZombie"))
            {
                other.GetComponent<DeafZombieController>().zombieHp -= CharacterManager.weaponDmg;
            }
            else if (other.gameObject.tag == "BossZombie")
            {
                other.GetComponent<BossZombieController>().zombieHp -= CharacterManager.weaponDmg;
            }

            Destroy(this.gameObject);
        }
        
    }
}
