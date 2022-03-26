using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Player : MonoBehaviour
{
    [SerializeField] private float angleRotation = 20;

    private LaserLevel levelManager;
    private RotationBoundry Boundry = null;

    private Vector2 mouseAnchor = Vector2.zero;

    private float midValue = 180f;  // Used to check player y rotation (To not exceed boundry values)
    private bool locked = false;

    private void Awake()
    {
        levelManager = (LaserLevel)LevelManager.Instance;
        Boundry = CreateBoundry();
    }
    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        // If game is started and player doesnt send laser
        if (GameManager.Instance.State == GameManager.GameState.STARTED && !locked)
        {
            Vector2 dif = Vector2.zero;
            if (Input.GetMouseButtonDown(0))
            {
                mouseAnchor = Input.mousePosition;
            }
            // While player want to decide where should laser go
            else if (Input.GetMouseButton(0))
            {
                dif = (Vector2)Input.mousePosition - mouseAnchor;
                levelManager.CreateLaser(transform.position, transform.forward, false);
            }
            // If player decided where to send laser
            else if (Input.GetMouseButtonUp(0))
            {
                levelManager.CreateLaser(transform.position, transform.forward, true);
                locked = true;  // It lockes the input to player can't play after the shoot
            }
            AdjustRotation(dif.x);  // Rotates Player
        }
    }

    private void AdjustRotation(float dif)
    {
        float angleRot = 0;
        // when we turning right (+ y direction (rotation)) transform's y value of rotation is greater or equal to Boundry value
        if (dif < 0)
        {
            angleRot = -angleRotation;
        }
        // That means Player will rotate left (-x)
        else if (dif > 0)
        {
            angleRot = angleRotation;
        }
        if (transform.eulerAngles.y >= Boundry.Left || transform.eulerAngles.y <= Boundry.Right)
        {
            RotatePlayer(angleRot * Time.deltaTime * 20);
        }
        else if (transform.eulerAngles.y > Boundry.Right || transform.eulerAngles.y < Boundry.Left)
        {
            // if rotation of y is not in expected interval
            // then adjust its degree to Right Boundry
            if (transform.eulerAngles.y < midValue)
            {
                Vector3 rotationMax = transform.eulerAngles;
                rotationMax.y = Boundry.Right - 1; // due to 0.00001 difference i set it up to 59
                transform.eulerAngles = rotationMax;
            }
            // if rotation of y is not in expected interval
            // then adjust its degree to Left Boundry
            if (transform.eulerAngles.y > midValue)
            {
                Vector3 rotationMin = transform.eulerAngles;
                rotationMin.y = Boundry.Left;
                transform.eulerAngles = rotationMin;
            }
        }
        
    }

    private void RotatePlayer(float angleRot)
    {
        // Rotation of the Player
        Vector3 rotationVal = transform.eulerAngles;
        rotationVal.y += angleRot;
        transform.eulerAngles = rotationVal;
    }

    private RotationBoundry CreateBoundry()
    {
        // Setting boundry angles
        float leftBoundry = 300f;   // Not -60 because .eulerAngles return 300 instead of -60
        float rightBoundry = 60f;
        return new RotationBoundry(leftBoundry, rightBoundry);
    }

    // Player rotation boundry class
    private class RotationBoundry
    {
        private float leftRotation;
        private float rightRotation;

        public float Left
        {
            get
            {
                return leftRotation;
            }
        }
        public float Right
        {
            get
            {
                return rightRotation;
            }
        }
        public RotationBoundry(float leftRotation, float rightRotation)
        {
            this.leftRotation = leftRotation;
            this.rightRotation = rightRotation;
        }
    }
}
