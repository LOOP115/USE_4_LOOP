using System.Collections.Generic;
using UnityEngine;

public class CreateRandom : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();

    private void Start()
    {
        GameObject objCreate = objects[Random.Range(0, objects.Count)];
        Instantiate(objCreate, transform.position, Quaternion.identity);
    }
}
