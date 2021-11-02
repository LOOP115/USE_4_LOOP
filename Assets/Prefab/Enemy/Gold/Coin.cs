using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 20f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 80 * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.GetComponent<PlayerInfo>().ChangeMoney(10);
            Destroy(gameObject);
        }
    }
}
