using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private float destroyTime;
    private Vector3 offset;

    private TextMeshPro damageText;

    // Start is called before the first frame update
    void Awake()
    {
        offset = new Vector3(Random.Range(-0.4f,0.4f),0.9f,0);
        destroyTime = 2;
        damageText = GetComponent<TextMeshPro>();
        transform.localPosition += offset;
        Destroy(gameObject, destroyTime);
    }

    public void Initialise(int damage)
    {
        damageText.text = damage.ToString();
    }
  
}
