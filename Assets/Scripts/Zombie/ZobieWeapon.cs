using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZobieWeapon : MonoBehaviour
{
    public int damage = 20;
    public float speed = 30.0f;

    private Rigidbody rigid;
    [SerializeField] GameObject zombieVfx;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        rigid.velocity = transform.forward * speed;

        Destroy(gameObject, 5.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            CharacterManager.instance.characterHp -= damage;
        }

        Instantiate(zombieVfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


}
