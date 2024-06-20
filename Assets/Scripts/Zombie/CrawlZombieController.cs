using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CrawlZombieController : MonoBehaviour
{
    public float zombieHp = 100.0f;
    public float zombieDamage = 10.0f;
    public float speed = 1.0f;

    Animator _zombieAnimator;

    private Vector3 startPos;
    public Transform targetTr;
    //시야 check 범위
    public float range = 20.0f;
    public float attackRange = 1.0f;
    float checkRange;

    bool isChecked = false;
    public bool isDead = false;
    bool isAttacking = false;
    float canAttackTerm = 1.0f;

    float inRangeTime;
    float outRangeTime;

    CapsuleCollider zombieCo;
    [SerializeField] AudioClip[] zombieIdleClips;
    AudioSource audioSource;

    //Nav Mesh
    protected NavMeshAgent nav;
    protected Vector3 destination;
    //UI

    // Start is called before the first frame update
    private void Awake()
    {
        startPos = transform.position;
        targetTr = GameObject.Find("Geometry").transform;
    }

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        _zombieAnimator = GetComponent<Animator>();
        zombieCo = GetComponent<CapsuleCollider>();
        nav.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(zombieHp > 0.0f) ZombieControl();
        else if (zombieHp <= 0.0f && isDead == false)
        {
            _zombieAnimator.speed = 0;
            ZombieDie();
        }
        canAttackTerm -= Time.deltaTime;
    }
    
    void IsPlayerInRange()
    {
        if (playerChecked)
        {
            inRangeTime += Time.deltaTime;
            outRangeTime = 0.0f;
        }
        else
        {
            inRangeTime = 0.0f;
            outRangeTime += Time.deltaTime;
        }
        if (outRangeTime >= 3)
        {
            isChecked = false;
        }

    }

    RaycastHit zombieHitInfo;
    bool playerChecked;
    void CheckPlayer()
    {
        Vector3 zombieRay = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        Vector3 targetRay = new Vector3(targetTr.transform.position.x,
            targetTr.transform.position.y + 1.0f, targetTr.transform.position.z);

        if (Physics.Raycast(zombieRay, targetRay - zombieRay, out zombieHitInfo, range + 5.0f))
        {
            if (zombieHitInfo.transform.name == "Player")
            {
                playerChecked = true;
            }
            else playerChecked = false;
        }
        IsPlayerInRange();
    }


    void ZombieControl()
    {
        float distance = Vector3.Distance(transform.position, targetTr.position);
        float soundDistance = Vector3.Distance(transform.position, ThirdPersonShooterController.soundPos);
        if (isDead == false && canMove)
        {
            //총소리에 따른 좀비 인식거리 변경
            if (ThirdPersonShooterController.isShooting)
                checkRange = range * 2;
            else
                checkRange = range;

            //총소리 나는 곳으로 이동
            if (soundDistance <= checkRange && ThirdPersonShooterController.isShooting)
            {
                nav.SetDestination(ThirdPersonShooterController.soundPos);
                _zombieAnimator.speed = 1.0f;
                _zombieAnimator.SetBool("isCrawl", true);
                _zombieAnimator.speed = speed;
            }else _zombieAnimator.speed = 0.0f;

            if (distance <= checkRange)
            {
                CheckPlayer();
                if (playerChecked)
                {
                    isChecked = true;

                    /*destination = new Vector3(transform.position.x - targetTr.position.x,
                        0f, transform.position.z - targetTr.position.z).normalized;*/
                    nav.SetDestination(targetTr.position);

                    _zombieAnimator.speed = 1.0f;
                    _zombieAnimator.SetBool("isCrawl", true);
                    _zombieAnimator.speed = speed;
                }else _zombieAnimator.speed = 0.0f;
            }
            if (isChecked == true)
            {
                CheckPlayer();
                nav.SetDestination(targetTr.position);
            }
            // 공격
            if (distance <= attackRange)
            {
                if (!isAttacking && canAttackTerm < 0.0f)
                {
                    StartCoroutine(Attack());
                }
            }

        }
        
    }
    private bool canMove = true;
    IEnumerator Attack()
    {
        isAttacking = true;
        _zombieAnimator.speed = 0;
        nav.enabled = false;
        canMove = false;
        targetTr.GetComponentInParent<StarterAssets.ThirdPersonController>().MoveSpeed = 0.1f;

        yield return new WaitForSeconds(2.0f);

        canMove = true;
        canAttackTerm = 1.0f;
        nav.enabled = true;
        _zombieAnimator.speed = 1;
        targetTr.GetComponentInParent<StarterAssets.ThirdPersonController>().MoveSpeed = 4.0f;
        isAttacking = false;
    }
    private void ZombieDie()
    {
        StopAllCoroutines();
        isDead = true;
        nav.enabled = false;
        zombieCo.radius = 0.001f;
        _zombieAnimator.speed = 0.0f;
        targetTr.GetComponentInParent<StarterAssets.ThirdPersonController>().MoveSpeed = 4.0f;
        Destroy(gameObject, 180.0f);
    }
}
