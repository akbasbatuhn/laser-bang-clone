using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Laser : MonoBehaviour
{
    [SerializeField] private GameObject laserPartPrefab;
    private LaserLevel levelManager;

    private float maxLength = 0.5f; // Max length of reflected laser (For laser preview part)
    public GameObject LaserPartPrefab
    {
        get
        {
            return laserPartPrefab;
        }
    }

    private void Awake()
    {
        levelManager = (LaserLevel)LevelManager.Instance;
        //transform.position = levelManager.GetPlayerPosition();
    }

    public void SendRay(Vector3 pos, Vector3 dir)
    {
        if (Physics.Raycast(pos, dir, out RaycastHit hit, 100))
        {
            // Find the type of object that laser hits
            Wall wall = hit.transform.GetComponent<Wall>();
            Mirror mirror = hit.transform.GetComponent<Mirror>();
            Target target = hit.transform.GetComponent<Target>();

            // Create laser (cylinder)
            GameObject laserPart = Instantiate(LaserPartPrefab, pos, Quaternion.LookRotation(dir), levelManager.Laser.transform);

            //Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.blue, 3f);

            // If laser hits Mirror, call SendRay() to continue reflection
            laserPart.LeanScaleZ(hit.distance, 0.2f).setOnComplete(()=> {
                if (mirror)
                {
                    SendRay(hit.point, Vector3.Reflect(dir, hit.normal));
                }
                // If not finish level
                else
                {
                    LeanTween.delayedCall(0.3f, () =>
                    {
                        levelManager.FinishLevel(target != null);   // If laser hits Target, return true; if not return false
                    });
                }
            });
        }
    }

    public void LaserPreview(Vector3 pos, Vector3 dir)
    {
        Ray ray = new Ray(pos, dir);
        if (Physics.Raycast(ray, out RaycastHit hit))   // Send ray
        {
            Mirror mirror = hit.transform.GetComponent<Mirror>();   // If ray's target is not type of Mirror, mirror will be null
            GameObject laserPart = Instantiate(laserPartPrefab, pos, Quaternion.LookRotation(dir), levelManager.Laser.transform);

            // Adjustment of laser's (cylinder) z rotation
            Vector3 laserScale = laserPart.transform.localScale;
            laserScale.z = hit.distance;
            laserPart.transform.localScale = laserScale;

            // if ray hits Mirror show another laser to where to reflect
            if (mirror)
            {
                ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
                GameObject secondLaserPart = Instantiate(laserPartPrefab, hit.point, Quaternion.LookRotation(ray.direction), levelManager.Laser.transform);
                float previewLength = maxLength;    // max length of reflected laser
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.distance < previewLength)
                    {
                        previewLength = hit.distance;   // if distance is less than desired length, make length distance
                    }
                    // Rotation adjustment of reflected laser
                    Vector3 secondLaserPreview = secondLaserPart.transform.localScale;
                    secondLaserPreview.z = previewLength;
                    secondLaserPart.transform.localScale = secondLaserPreview;
                }
            }
        }
        // To prevent laser stack destroy laser object
        LeanTween.delayedCall(0.001f, () =>
        {
            Destroy(levelManager.Laser.gameObject);
        });
    }
}
