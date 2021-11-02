using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 100;
    public int playerHealth;
    public Text healthText;
    public Text scoreText;
    public int playerScore;

    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip showInfoSound;
    [SerializeField] private GameObject showInfo;
    [SerializeField] private Text showInfoText;
    [SerializeField] private AudioSource audioSource;

    private int damage;
    private int money = 0;
    private float scoreAddingTime = 0.01f;
    private float time = 0;
    private int preScore = 0;
    private float canHealing = 0;
    private float healingRate = 0;

    // Start is called before the first frame update
    void Start()
    {
        damage = 20;
        playerHealth = maxHealth;
        playerScore = 0;
        healthBar.SetMaxHealth(playerHealth);
        healthBar.SetHealth(playerHealth);
        healthText.text = "" + maxHealth;
        scoreText.text = "Score: " + playerScore;
    }


    // Update is called once per frame
    void Update()
    {
        healthBar.SetHealth(playerHealth);
        healthText.text = "" + playerHealth;
        if(time<=0 && playerScore != preScore)
        {
            preScore += 2;
            scoreText.text = "Score: " + preScore;
            time = scoreAddingTime;
        }
        if (canHealing<=0 && healingRate <=0 && playerHealth != maxHealth)
        {
            playerHealth += 1;
            healingRate = 0.1f;
        }

        time -= Time.deltaTime;
        healingRate -= Time.deltaTime;
        canHealing -= Time.deltaTime;
    }

    public void LogInfo(string info)
    {
        StartCoroutine(ShowInfo(info));
    }

    private IEnumerator ShowInfo(string info)
    {
        showInfoText.text = info;
        showInfo.SetActive(true);
        audioSource.clip = showInfoSound;
        audioSource.Play();
        yield return new WaitForSeconds(2.5f);
        showInfo.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        healthBar.SetHealth(playerHealth);
        canHealing = 6f;
    }

    public void AddScore(int score)
    {
        playerScore += score;
    }

    public void ChangeMoney(int value)
    {
        AudioSource.PlayClipAtPoint(coinSound,transform.position);
        money += value;
    }
    public void IncreaseHealth(int value)
    {
        maxHealth += value;
        playerHealth = maxHealth;
        healthBar.SetMaxHealth(playerHealth);
    }
    public int GetDamage()
    {
        return damage;
    }
    public void IncreaseDamage(int value)
    {
        damage += value;
    }
    public int GetMoney()
    {
        return money;
    }
}
