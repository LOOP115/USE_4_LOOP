using UnityEngine;

public class GroundCheck : MonoBehaviour
{
     float distancetoGround;
    public float ObjectPivot;
    // Start is called before the first frame update
    void Start()
    {
        distancetoGround = GetComponent<Collider>().bounds.extents.y;
        RaycastHit hit;
      Physics.Raycast(transform.position, -Vector3.up,out hit,1500);
        print(hit.transform.position);
        transform.position =  new Vector3(transform.position.x, hit.transform.position.y + ObjectPivot, transform.position.z);
    }

// Update is called once per frame
void Update()
    {
        
    }
}
