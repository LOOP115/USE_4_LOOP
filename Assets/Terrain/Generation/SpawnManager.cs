using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] _objs;
    void Start()
    {
        int rando = Random.Range(0, _objs.Length);
        GameObject currProp = Instantiate(_objs[rando], transform.position += new Vector3(0, 0.75f), Quaternion.identity);
        currProp.GetComponent<Transform>().parent = transform;
    }

        // Update is called once per frame
        void Update()
    {
        
    }
}
