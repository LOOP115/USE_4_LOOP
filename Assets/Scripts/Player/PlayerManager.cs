using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton

    public static PlayerManager instance;

    private void Awake()
    {
        Time.timeScale = 1f;
        instance = this;
        player = GameObject.Find("FPP_Player");
    }

    #endregion

    public GameObject player;
}
