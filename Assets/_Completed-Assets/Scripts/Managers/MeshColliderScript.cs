using UnityEngine;

// Script for adding mesgh colldiers to enviorment
public class MeshColliderScript : MonoBehaviour
{
    void OnEnable()
    {
        // Finds all child game objects
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Enviorment");
        // Adds a MeshColldier to all child objects
        foreach (GameObject geo in obj)
            geo.AddComponent<MeshCollider>();
    }
}
