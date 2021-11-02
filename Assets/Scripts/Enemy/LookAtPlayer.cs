using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{

    private void Update()
    {
        transform.position = PlayerManager.instance.player.transform.localPosition - new Vector3(0,1,0);
    }
}
