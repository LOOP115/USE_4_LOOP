using UnityEngine;

public class FacingUI : MonoBehaviour
{
    private Camera playerCam;

    private void Start()
    {
        playerCam = PlayerManager.instance.player.transform.GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        LookToPlayer();
    }

    private void LookToPlayer()
    {
        transform.LookAt(transform.position + playerCam.transform.rotation * Vector3.forward, playerCam.transform.rotation*Vector3.up);
    }
}
