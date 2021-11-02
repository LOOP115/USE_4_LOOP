using UnityEngine;

public class ForestCreator : MonoBehaviour
{
    [SerializeField] private int forestSize = 150;
    [SerializeField] private int spacing = 5;
    [SerializeField] private int maxTreeHeightPos = 7;
    [SerializeField] private GameObject treePrefab;

    // Start is called before the first frame update
    void Awake()
    {
        Vector3 origin = transform.position;
        for (int x=0; x<forestSize; x+=spacing)
        {
            for (int y=0; y<forestSize; y+=spacing)
            {
                var offset = new Vector3(Random.Range(-6f, 6f), 0, Random.Range(-6f, 6f));
                var newTreePostion = EnemyManager.GetGroundPostion(new Vector3(x, 0f, y) + origin + offset);
                if (newTreePostion.y - origin.y < maxTreeHeightPos)
                {
                    var newTree = Instantiate(treePrefab);
                    newTree.transform.position = newTreePostion;
                }
                
            }
        }

    }
}
