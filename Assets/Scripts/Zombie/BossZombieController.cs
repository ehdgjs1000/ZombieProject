using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossZombieController : MonoBehaviour
{
    public float zombieMaxHp;
    public float zombieHp;
    public float zombieDamage;

    Animator _zombieAnimator;
    Rigidbody rigid;

    public Transform targetTr;
    //시야 check 범위
    public float range = 10.0f;
    public float attackRange = 0.3f;
    float checkRange;
    [SerializeField] float dotDamage = 0.1f;
    [SerializeField] float speed = 0.3f;


    bool isChecked = false;
    public bool isDead = false;
    bool isAttacking = false;
    float canAttackTerm = 1.0f;

    float inRangeTime;
    float outRangeTime;

    //원거리 공격
    [SerializeField] GameObject zombieThrowingPrefab;
    [SerializeField] Transform spawnThrowingPos;
    [SerializeField] Transform screamPos;
    [SerializeField] GameObject fireGo;

    CapsuleCollider zombieCo;
    [SerializeField] AudioClip[] zombieIdleClips;
    AudioSource audioSource;

    //spawn Zombie
    public Collider[] colliders;
    private float radius = 150.0f;
    private Vector3 bossStartPos;
    [SerializeField] LayerMask zombieLayer;

    //Nav Mesh
    protected NavMeshAgent nav;
    protected Vector3 destination;

    public int[] patternNum = { 1, 1, 2, 1, 2, 3, 1, 4 };
    private int patternCount = 0;
    //UI
    [SerializeField] private ParticleSystem poisionVfx;

    // Start is called before the first frame update
    private void Awake()
    {
        //Prefab으로 할 경우 캐릭터 찾아오기 위함
        targetTr = GameObject.Find("Geometry").transform;

        bossStartPos = transform.position;
    }

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        _zombieAnimator = GetComponent<Animator>();
        zombieCo = GetComponent<CapsuleCollider>();
        rigid = GetComponent<Rigidbody>();
        nav.speed = speed;
        zombieHp = zombieMaxHp;
    }

    // Update is called once per frame
    void Update()
    {
        ZombieControl();
        CheckPlayerRange();
        if (zombieHp <= 0.0f && isDead == false)
        {
            _zombieAnimator.SetBool("isDead", true);
            ZombieDie();
        }

        canAttackTerm -= Time.deltaTime;

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
            }
            if (distance <= checkRange)
            {
                CheckPlayer();
                if (playerChecked && canUseNavMesh)
                {
                    //audioSource.PlayOneShot(zombieIdleClips[0]);
                    isChecked = true;

                    destination = new Vector3(transform.position.x - targetTr.position.x,
                        0f, transform.position.z - targetTr.position.z).normalized;
                    nav.SetDestination(targetTr.position);

                    _zombieAnimator.SetBool("isRunning", true);
                }
            }
            // 공격
            if (distance <= attackRange)
            {
                if (!isAttacking && canAttackTerm < 0.0f)
                {
                    //pattern 공격
                    int a = patternNum[patternCount];
                    patternCount++;
                    if (patternCount >= patternNum.Length) patternCount = 0;

                    //int a = Random.Range(0, 11);
                    if (a == 1)
                    {
                        StartCoroutine(LongAttack1());
                    }
                    else if (a == 2)
                    {
                        StartCoroutine(LongAttack2());
                    }
                    else if (a == 3)
                    {
                        StartCoroutine(LongAttack3());
                    }
                    else if (a == 4)
                    {
                        StartCoroutine(LongAttack4());
                    }
                    else if (a == 5)
                    {
                        //StartCoroutine(LongAttack5());
                    }

                }
            }
        }
       
        if (!CheckBossZone.inBossZone)
        {
            isChecked = false;      
            Invoke("BossReset", 1.0f);
        }

    }


    public void BossReset()
    {
        //transform.position = bossStartPos;
        //nav.ResetPath();
        nav.SetDestination(bossStartPos);
        _zombieAnimator.SetBool("isRunning", true);
        zombieHp = zombieMaxHp;
        playerChecked = false;
        isChecked = false;

        if (!playerChecked) Invoke("ResetNav", 10.0f);
    }
    private void ResetNav()
    {
        transform.position = bossStartPos;
        nav.ResetPath();
        _zombieAnimator.SetBool("isRunning", false);
    }

    //3가지 공격 type
    IEnumerator LongAttack1()
    {
        isAttacking = true;
        nav.speed = 0.0f;
        nav.ResetPath();

        transform.LookAt(targetTr.transform.position);
        _zombieAnimator.SetTrigger("isThrowing");
        transform.rotation = Quaternion.Euler(targetTr.position.x - spawnThrowingPos.position.x,
            targetTr.position.y - spawnThrowingPos.position.y,
            targetTr.position.z - spawnThrowingPos.position.z);

        yield return new WaitForSeconds(0.7f);
        Instantiate(zombieThrowingPrefab, spawnThrowingPos.position,
            Quaternion.LookRotation(new Vector3(targetTr.position.x - spawnThrowingPos.position.x,
            targetTr.position.y - spawnThrowingPos.position.y + 1f,
            targetTr.position.z - spawnThrowingPos.position.z).normalized, Vector3.up));
        yield return new WaitForSeconds(1.0f);

        canAttackTerm = 1.0f;
        nav.speed = speed;
        isAttacking = false;
    }

    IEnumerator LongAttack2()
    {
        isAttacking = true;
        nav.speed = 0.0f;
        nav.ResetPath();

        transform.LookAt(targetTr.transform.position);
        _zombieAnimator.SetTrigger("isScream");

        yield return new WaitForSeconds(0.5f);
        int ranSpawnNum = Random.Range(5, 10);
        for (int i = 0; i < ranSpawnNum; i++)
        {
            float randX = Random.Range(-3f, 3f);
            float randZ = Random.Range(-3f, 3f);
            Instantiate(zombieThrowingPrefab, screamPos.position,
            Quaternion.LookRotation(new Vector3(targetTr.position.x - screamPos.position.x + randX,
            targetTr.position.y - screamPos.position.y + 1f,
            targetTr.position.z - screamPos.position.z + randZ).normalized, Vector3.up));
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.7f);
        canAttackTerm = 1.0f;
        isAttacking = false;
        nav.speed = speed;
    }

    //체력 회복
    IEnumerator LongAttack3()
    {
        isAttacking = true;
        nav.speed = 0.0f;
        nav.ResetPath();

        _zombieAnimator.SetTrigger("isSpawn");
        zombieHp += zombieMaxHp / 3;

        //Call near zombie
        SearchNearZombie();

        yield return new WaitForSeconds(1.5f);

        canAttackTerm = 1.0f;
        isAttacking = false;
        nav.speed = speed;

    }
    private bool canUseNavMesh = true;
    IEnumerator LongAttack4()
    {
        isAttacking = true;
        nav.speed = 1.0f;
        nav.ResetPath();

        transform.LookAt(targetTr.transform.position);
        nav.enabled = false;
        canUseNavMesh = false;
        poisionVfx.gameObject.SetActive(false);
        _zombieAnimator.SetTrigger("Jump");
        yield return new WaitForSeconds(1.5f);

        //캐릭터 충돌 확인
        float dist = Vector3.Distance(transform.position, targetTr.position);
        if (dist <= 3.0f)
        {
            CharacterManager.instance.characterHp -= 20.0f;
        }

        poisionVfx.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        canUseNavMesh = true;
        nav.enabled = true;
        yield return new WaitForSeconds(0.5f);

        canAttackTerm = 1.0f;
        isAttacking = false;
        nav.speed = speed;

    }
    IEnumerator LongAttack5()
    {
        isAttacking = true;
        nav.enabled = false;

        transform.LookAt(targetTr.transform.position);
        canUseNavMesh = false;

        _zombieAnimator.SetTrigger("isScream");

        Instantiate(fireGo,screamPos.transform.position,Quaternion.identity);
        yield return new WaitForSeconds(3.2f);

        canUseNavMesh = true;
        nav.enabled = true;
        yield return new WaitForSeconds(0.5f);
        canAttackTerm = 1.0f;
        isAttacking = false;
        nav.speed = 1.0f;
    }
    void SearchNearZombie()
    {
        {
            colliders = Physics.OverlapSphere(transform.position, radius, zombieLayer);

            foreach (Collider col in colliders)
            {
                if (col.tag == "ShortZombie") col.GetComponent<ShortZombieController>().BossOrderAttack();
                else if (col.tag == "LongZombie") col.GetComponent<LongZombieController>().BossOrderAttack();
            }

        }
    }
    void CheckPlayerRange()
    {
        float distance = Vector3.Distance(transform.position, targetTr.position);
        if (distance <= 5.0f)
        {
            CharacterManager.instance.characterHp -= 0.5f * Time.deltaTime;
        }
    }

    private void ZombieDie()
    {
        nav.enabled = false;
        zombieCo.radius = 0.001f;
        isDead = true;

        Destroy(gameObject, 180.0f);
    }
}