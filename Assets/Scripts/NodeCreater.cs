using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCreater : MonoBehaviour
{
    [SerializeField]
    private GameObject floor;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] floorVerticies = floor.GetComponent<MeshFilter>().sharedMesh.vertices;

        for (float x = floorVerticies[0].x; x < floorVerticies[120].x; x += 0.5f)
        {
            for (float y = floorVerticies[0].y; y < floorVerticies[120].y; y += 0.5f)
            {

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
