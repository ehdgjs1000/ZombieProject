using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCtrl : MonoBehaviour
{
    [SerializeField] GameObject explosionVfx;
    public float damage = 100.0f;
    public float throwForce = 40f;
    public GameObject grenadePrefab;
    Rigidbody rb;
    private float explodeTime = 3.0f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        ThrowGrenade();

    }
    private void Update()
    {
        explodeTime -= Time.deltaTime;
        if (explodeTime <= 0.0f) Explode();
    }

    void ThrowGrenade()
    {
        rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }

    void Explode()
    {
        Instantiate(explosionVfx, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}