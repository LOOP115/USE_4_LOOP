using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth;
    public int enemyHealth;

    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    bool isAttack = false;
    bool isDead = false;

    private float attackCoolDown = 0;
    [SerializeField] private GameObject bar;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private GameObject criticleDamageTextPrefab;
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject explosionBloodPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float armor;
    [SerializeField] private float speed;
    [SerializeField] private float attckRange;
    [SerializeField] private float attackSpeed;

    [System.Serializable]
    public class SoundClips
    {
        public AudioClip attackSound;
        public AudioClip takeDamageSound1;
        public AudioClip takeDamageSound2;
        public AudioClip dieSound;
    }
    [SerializeField] private SoundClips soundClips;
    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;
    void Start()
    {

        enemyHealth = maxHealth;
        healthBar.SetMaxHealth(enemyHealth);
        healthBar.SetHealth(enemyHealth);
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.acceleration = 999;
        playerTransform = PlayerManager.instance.player.transform;

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (enemyHealth <= 0 && !isDead)
        {
            isDead = true;
            StopMoving();
            StartCoroutine(Dead());
        }
      
        if(enemyHealth > 0){
            attackCoolDown -= Time.deltaTime;
            if (distance <= attckRange && attackCoolDown <= 0 && !isAttack)
            {
                isAttack = true;
                StopMoving();
                StartCoroutine(Attack());
                attackCoolDown = attackSpeed;
            }

            if(!isAttack && distance > attckRange)
            {
                Walk();
            }
        }   
    }

    private void StopMoving()
    {
        navMeshAgent.speed = 0f;
        navMeshAgent.acceleration = 0f;
        navMeshAgent.velocity = new Vector3(0, 0, 0);
    }

    private void StartMoving()
    {
        navMeshAgent.speed = speed;
        navMeshAgent.acceleration = 999f;
    }

    public void TakeDamage(int damage, Vector3 position)
    {
        float randomNumber = Random.Range(0f, 2f);
        if (randomNumber >=1)
        {
            audioSource1.clip = soundClips.takeDamageSound1;
        }
        else
        {
            audioSource1.clip = soundClips.takeDamageSound2;
        }
        audioSource1.Play();
        bar.SetActive(true);
        damage = Mathf.RoundToInt((damage + Random.Range(-5, 5)) * (1f - armor));
        enemyHealth -= damage;
        if (damage > 50)
        {
            Instantiate(criticleDamageTextPrefab, transform.position, Quaternion.identity).GetComponent<CriticleText>().Initialise(damage);
            if(enemyHealth<=0)
                Instantiate(explosionBloodPrefab, position - new Vector3(0, 0.5f, 0), Quaternion.identity);
            else
                Instantiate(bloodPrefab, position, Quaternion.identity);
        }
        else
        {
            Instantiate(damageTextPrefab, transform.position, Quaternion.identity).GetComponent<DamageText>().Initialise(damage);
            Instantiate(bloodPrefab, position, Quaternion.identity);
        }
        healthBar.SetHealth(enemyHealth);

    }

    private void Walk()
    {
        StartMoving();
        navMeshAgent.SetDestination(playerTransform.position);
        anim.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
    }

    private IEnumerator Attack()
    {
        anim.SetTrigger("Attack");
        audioSource1.PlayOneShot(soundClips.attackSound);
        yield return new WaitForSeconds(1f);
        isAttack = false;
    }

    private IEnumerator Dead()
    {
        audioSource2.Play();
        playerTransform.GetComponent<PlayerInfo>().AddScore(100);
        anim.SetTrigger("Dead");
        EnemyManager.totalNumberZombieAlive--;
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;
        Instantiate(coinPrefab, transform.localPosition + new Vector3(0,0.3f,0), Quaternion.identity);
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public bool IsAttacking()
    {
        return isAttack;
    }
}
