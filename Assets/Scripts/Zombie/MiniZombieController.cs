using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MiniZombieController : MonoBehaviour
{
    public float zombieHp = 50.0f;
    public float zombieDamage = 5.0f;
    public bool longRange;

    Animator _zombieAnimator;

    public Transform targetTr;
    //시야 check 범위
    public float range = 10.0f;
    public float attackRange = 0.3f;

    public bool isDead = false;
    bool isAttacking = false;
    float canAttackTerm = 1.0f;

    [SerializeField] AudioClip[] zombieIdleClips;
    AudioSource audioSource;

    //Nav Mesh
    protected NavMeshAgent nav;
    protected Vector3 destination;
    //UI

    // Start is called before the first frame update
    private void Awake()
    {
        //Prefab으로 할 경우 캐릭터 찾아오기 위함
        targetTr = GameObject.Find("Geometry").transform;
    }

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        _zombieAnimator = GetComponent<Animator>();
        nav.speed = 1.5f;
        Destroy(gameObject, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        ZombieControl();

        if (zombieHp <= 0.0f && isDead == false)
        {
            _zombieAnimator.SetBool("isDead", true);
            ZombieDie();
        }

        canAttackTerm -= Time.deltaTime;

    }

    void ZombieControl()
    {
        float distance = Vector3.Distance(transform.position, targetTr.position);
        if (isDead == false)
        {
            _zombieAnimator.SetBool("isRunning", true);
            nav.SetDestination(targetTr.position);

            if (distance <= attackRange)
            {
                if (!isAttacking && canAttackTerm < 0.0f)
                {
                    StartCoroutine(Attack());
                }
            }

        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        if (!longRange)
        {
            _zombieAnimator.SetBool("isAttack", true);
        }
        yield return new WaitForSeconds(1.0f);
        canAttackTerm = 1.0f;
        isAttacking = false;
    }

    void ZombieDie()
    {
        nav.enabled = false;
        isDead = true;
        Destroy(gameObject,2.0f);
    }
    
    void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.tag == "Player" && isAttacking && canAttackTerm < 0.0f)
        {
            canAttackTerm = 1.0f;
            CharacterManager.instance.characterHp -= zombieDamage;
        }
    }

}
