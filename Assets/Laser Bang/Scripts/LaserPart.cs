using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class LaserPart : MonoBehaviour
{
    // Collectibles
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Collectible"))
        {
            Destroy(other.gameObject);
        }
    }
}
