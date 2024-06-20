using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LongZombieController : MonoBehaviour
{
    public float zombieHp = 100.0f;
    public float zombieDamage;
    public float speed;

    Animator _zombieAnimator;

    private Vector3 startPos;
    public Transform targetTr;
    //시야 check 범위
    public float range;
    public float attackRange = 0.3f;
    float checkRange;

    bool isChecked = false;
    public bool isDead = false;
    bool isAttacking = false;
    float canAttackTerm = 1.0f;

    float inRangeTime;
    float outRangeTime;

    //원거리 공격
    [SerializeField] GameObject zombieThrowingPrefab;
    [SerializeField] private Transform spawnThrowingPos;

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
        //Prefab으로 할 경우 캐릭터 찾아오기 위함
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
        ZombieControl();

        if (zombieHp <= 0.0f && isDead == false)
        {
            _zombieAnimator.SetBool("isDead", true);
            ZombieDie();
        }

        canAttackTerm -= Time.deltaTime;

    }

    public void BossOrderAttack()
    {
        if (zombieHp > 0)
        {
            nav.SetDestination(targetTr.position);
            _zombieAnimator.SetBool("isRunning", true);
        }

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
        if (isDead == false)
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
                _zombieAnimator.SetBool("isRunning", true);
                _zombieAnimator.speed = speed;
            }

            if (distance <= checkRange)
            {
                CheckPlayer();
                if (playerChecked)
                {
                    isChecked = true;
                    /*destination = new Vector3(transform.position.x - targetTr.position.x,
                        0f, transform.position.z - targetTr.position.z).normalized;*/
                    nav.SetDestination(targetTr.position);

                    _zombieAnimator.SetBool("isRunning", true);
                    _zombieAnimator.speed = speed;
                }
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
                    StartCoroutine(LongAttack());
                }
            }

        }
    }
    IEnumerator LongAttack()
    {
        isAttacking = true;
        nav.ResetPath();
        nav.speed = 0f;

        transform.LookAt(targetTr.position);
        _zombieAnimator.SetTrigger("isThrowing");
        yield return new WaitForSeconds(0.8f);
        Instantiate(zombieThrowingPrefab, spawnThrowingPos.position,
            Quaternion.LookRotation(new Vector3(targetTr.position.x - spawnThrowingPos.position.x,
            targetTr.position.y - spawnThrowingPos.position.y + 1f,
            targetTr.position.z - spawnThrowingPos.position.z).normalized, Vector3.up));
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
        nav.speed = speed;
    }

    private void ZombieDie()
    {
        isDead = true;
        nav.enabled = false;
        zombieCo.radius = 0.001f;

        Destroy(gameObject, 180.0f);
    }
    void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.tag == "Player" && isAttacking && canAttackTerm < 0.0f)
        {
            canAttackTerm = 1.0f;
            CharacterManager.instance.characterHp -= zombieDamage *
                (1 - (EquipSlotManager.helmetLv * 5f / 100) - (EquipSlotManager.armorLv * 5f / 100));

            //디버프
            CharacterDebuff();
        }
    }
    void CharacterDebuff()
    {
        int duffProbabillity = Random.Range(0, 100);
        if (CharacterManager.instance.debuffType[0] == false &&
           CharacterManager.instance.debuffType[1] == false)
        {
            if (duffProbabillity <= 100) // 디버프 추가 
            {
                int debuffType = Random.Range(0, 2);
                switch (debuffType)
                {
                    case 0:
                        //긁힘
                        CharacterManager.instance.debuffType[0] = true;
                        break;
                    case 1:
                        //출혈
                        CharacterManager.instance.debuffType[1] = true;
                        break;
                }
            }
        }
        else if (CharacterManager.instance.debuffType[0] == true ||
           CharacterManager.instance.debuffType[1] == true)
        {
            if (duffProbabillity <= 100) // 디버프 추가 
            {
                int debuffType = Random.Range(0, 2);
                switch (debuffType)
                {
                    case 0:
                        //어지러움
                        CharacterManager.instance.debuffType[2] = true;
                        break;
                    case 1:
                        //환각
                        CharacterManager.instance.debuffType[3] = true;
                        break;
                }
            }
        }




    }

}
