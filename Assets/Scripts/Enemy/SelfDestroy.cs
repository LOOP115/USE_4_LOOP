using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    private float destroyTime;

    // Start is called before the first frame update
    void Awake()
    {
        destroyTime = 2;
        Destroy(gameObject, destroyTime);
    }

}
