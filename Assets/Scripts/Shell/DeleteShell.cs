using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteShell : MonoBehaviour
{
    // Immidiately destroys the game object
    public void Despawn()
    {
        Destroy(gameObject);
    }
}
