using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class LaserLevel : LevelManager
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject coinPrefab;

    private Player player;
    private Laser laser = null;

    public Laser Laser
    {
        get
        {
            return laser;
        }
    }
    public Player Player
    {
        get
        {
            return player;
        }
    }
    // Creates laser (GameObject that includes laserParts)
    public void CreateLaser(Vector3 pos, Vector3 dir, bool isReady)
    {
        //coinPrefab.GetComponent<BoxCollider>().isTrigger = isReady;

        laser = Instantiate(laserPrefab, pos, Quaternion.LookRotation(dir)).GetComponent<Laser>();
        // If player wants to send ray
        if (isReady)
        {
            coinPrefab.GetComponent<BoxCollider>().enabled = true;
            print(coinPrefab.GetComponent<BoxCollider>().enabled);
            laser.SendRay(pos, dir);
        }
        // If player wants to see laser preview
        else
        {
            laser.LaserPreview(pos, dir);
        }
    }

    public GameObject LaserPrefab
    {
        get
        {
            return laserPrefab;
        }
    }

    private new void Awake()
    {
        base.Awake();
        player = FindObjectOfType<Player>();
    }
    public override void FinishLevel(bool success)
    {
        GameManager.Instance.FinishLevel(success);
        //coinPrefab.GetComponent<BoxCollider>().enabled = false;
    }

    public override void StartLevel()
    {
        coinPrefab.GetComponent<BoxCollider>().enabled = false;
        print("Game Started ! ");
    }
}
