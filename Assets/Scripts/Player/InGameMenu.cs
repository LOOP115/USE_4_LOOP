using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public static bool isPause = false;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject gameInfo;
    [SerializeField] private GameObject deadMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private Text score;
    [SerializeField] private Text gold;
    [SerializeField] private Text speedText;
    [SerializeField] private Text damageText;

    private int damage = 20;
    private float speed = 4f;
    private bool playerDead = false;
    private bool isInShop = false;
    private bool isInPauseMenu = false;
    private Transform playerTrans;
    private PlayerInfo playerInfo;
    private ControllerScript playerController;
    private GunScript playerGun;
    private Quaternion currentPlayerRotation;

    public void QuitGame()
    {
        InGameMenu.isPause = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void Restart()
    {
        InGameMenu.isPause = false;
        deadMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Start()
    {
        playerTrans = PlayerManager.instance.player.transform;
        playerInfo= playerTrans.GetComponent<PlayerInfo>();
        playerController = playerTrans.GetComponent<ControllerScript>();
        playerGun = playerTrans.GetComponentInChildren<GunScript>();
    }

    private void Update()
    {
        gold.text = "Gold: " + playerTrans.GetComponent<PlayerInfo>().GetMoney();
        if(playerTrans.GetComponent<PlayerInfo>().playerHealth <= 0)
        {
            playerDead = true;
            isPause = true;
            score.text = "Your Score: " + playerTrans.GetComponent<PlayerInfo>().playerScore;
            playerTrans.GetComponent<PlayerInfo>().scoreText.text = "Score: " + playerTrans.GetComponent<PlayerInfo>().playerScore;
            deadMenu.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !playerDead)
        {
            shopMenu.SetActive(false);
            if (isPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !playerDead)
        {
                OpenShop();       
        }
    }

    public void Resume()
    {
        playerTrans.GetComponent<ControllerScript>().SetCameraRoation(true);
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);
        gameInfo.SetActive(false);
        Time.timeScale = 1f;
        isPause = false;
        isInPauseMenu = false;
    }
    private void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        pauseMenuUI.SetActive(true);
        gameInfo.SetActive(true);
        Time.timeScale = 0f;
        isPause = true;
        isInPauseMenu = true;
    }

    private void OpenShop()
    {
        if (isInShop && !isInPauseMenu)
        {
            playerTrans.GetComponent<ControllerScript>().SetCameraRoation(true);
            shopMenu.SetActive(false);
            isInShop = false;
            Cursor.lockState = CursorLockMode.Locked;
            isPause = false;
            isInPauseMenu = false;
        }
        else if(!isInPauseMenu)
        {
            playerTrans.GetComponent<ControllerScript>().SetCameraRoation(false);
            shopMenu.SetActive(true);
            isInShop = true;
            Cursor.lockState = CursorLockMode.None;
            //Time.timeScale = 0f;
            isPause = true;
            isInPauseMenu = false;
        }
    }

    public void SellSoul()
    {
        if (playerInfo.GetMoney() >= 10)
        {
            playerInfo.ChangeMoney(playerInfo.GetMoney());
            playerInfo.LogInfo("Money doubled, your soul become dirty !!!!!");
        }
        else
        {
            playerInfo.LogInfo("No money to Sell Your soul");
        }
    }

    public void SpeedUp()
    {
        if (playerInfo.GetMoney() >= 230)
        {
            playerController.IncreaseSpeed(0.3f);
            speedText.text = "Speed: " + Mathf.Round(speed*100f) / 100f;
            playerInfo.LogInfo("Speed UP ++");
            playerInfo.ChangeMoney(-230);
            speed += 0.3f;
        }
        else
        {
            playerInfo.LogInfo("Not enough Gold, Kill more!");
        }
    }

    public void HealthUp()
    {
        if (playerInfo.GetMoney() >= 300)
        {
            playerInfo.IncreaseHealth(50);
            playerInfo.LogInfo("Max Health UP ++");
            playerInfo.ChangeMoney(-300);
        }
        else
        {
            playerInfo.LogInfo("Not enough Gold, Kill more!");
        }
    }

    public void AmmoUp()
    {
        if (playerInfo.GetMoney() >= 50)
        {
            playerGun.IncreaseAmmo(10);
            playerInfo.LogInfo("Ammo UP ++");
            playerInfo.ChangeMoney(-50);
        }
        else
        {
            playerInfo.LogInfo("Not enough Gold, Kill more!");
        }
    }

    public void DamageUp()
    {
        if (playerInfo.GetMoney() >= 100)
        {
            playerInfo.IncreaseDamage(5);
            playerInfo.LogInfo("Damage UP ++");
            damageText.text = "Damage: " + damage;
            damage += 5;
            playerInfo.ChangeMoney(-100);
        }
        else
        {
            playerInfo.LogInfo("Not enough Gold, Kill more!");
        }
    }

    public void GrenadeUp()
    {
        if (playerInfo.GetMoney() >= 20)
        {
            playerGun.IncreaseGrenade(5);
            playerInfo.LogInfo("Grenade ++");
            playerInfo.ChangeMoney(-20);
        }
        else
        {
            playerInfo.LogInfo("Not enough Gold, Kill more!");
        }
    }

    public void BarrelUp()
    {
        if (playerInfo.GetMoney() >= 50)
        {
            playerGun.IncreaseBarrel(5);
            playerInfo.LogInfo("Barrel ++");
            playerInfo.ChangeMoney(-50);
        }
        else
        {
            playerInfo.LogInfo("Not enough Gold, Kill more!");
        }
    }

    public void AttackSpeedUp()
    {
        if (playerInfo.GetMoney() >= 100)
        {
            playerGun.IncreaseFireRate(0.5f);
            playerInfo.LogInfo("Attack Speed Up ++");
            playerInfo.ChangeMoney(-100);
        }
        else
        {
            playerInfo.LogInfo("Not enough Gold, Kill more!");
        }
    }
}
