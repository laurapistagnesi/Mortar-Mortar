using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System;

public class ShootBehaviour : MonoBehaviour
{
    [SerializeField]
    private float cannonAdditionalRotationAngle = 60f;

    [SerializeField]
    private Transform cannonTubeTransform = null;

    [SerializeField]
    private AnimationCurve shootAngleOverTime = AnimationCurve.EaseInOut(0f, 0f, 90f, 1f);

    [SerializeField]
    private UnityEvent onShoot = null;

    public Transform pivotTransform;
    [SerializeField]
    public LineBehaviour lineBehaviour;
    private float shootAngle;
    private bool loadingShot;
    private float loadingShotStartTime;
    private int currentBulletIndex = 0;
    private Quaternion cannonAdditionalRotation;

    [SerializeField] private float force = 20.0f; // Forza di lancio
    [SerializeField] private float swipeThreshold = 10.0f; // La soglia per considerare uno swipe
    private Vector3 swipeStartPos;
    private Vector3 swipeEndPos;
    [SerializeField] private bool canShoot = false;
    [SerializeField] private GameObject panelWaiting;
    public TowerManager towerManager;
    [SerializeField] public TextMeshProUGUI remainingText;
    public float rotationSpeed = 5;
    bool isPressedRight;
    bool isPressedLeft;

    void Start()
    {
        AutoPlacement.OnTowerPlaced += OnTowerPlaced;
        shootAngle = -shootAngleOverTime.Evaluate(0f);
        cannonAdditionalRotation = Quaternion.AngleAxis(cannonAdditionalRotationAngle, Vector3.right);
    }

    void Update()
    {
        if (canShoot)
        {
            panelWaiting.SetActive(false);
            float yDirection = Input.GetAxis("Horizontal");
            RotatePivot(yDirection);

            UpdateShootInput();

            lineBehaviour.SetActive(true);
            UpdateProjectileDirection();
        }
        if(isPressedRight)
        {
            int yRotation = Mathf.RoundToInt(pivotTransform.rotation.eulerAngles.y);
            if ((16f < yRotation) && (yRotation < 345f))
            {
                return;
            }
            pivotTransform.rotation *= Quaternion.Euler(0.0f, rotationSpeed * Time.deltaTime*(-1.5f), 0.0f);

        }
        if(isPressedLeft)
        {
            int yRotation = Mathf.RoundToInt(pivotTransform.rotation.eulerAngles.y);
            if ((15f < yRotation) && (yRotation < 344f))
            {
                return;
            }
            pivotTransform.rotation *= Quaternion.Euler(0.0f, rotationSpeed * Time.deltaTime*1.5f, 0.0f);
        }
    }
    private void OnTowerPlaced()
    {
        canShoot = true;
    }
    private void UpdateProjectileDirection()
    {
        var localShootDirection = Quaternion.AngleAxis(shootAngle, Vector3.right);
        cannonTubeTransform.localRotation = localShootDirection * cannonAdditionalRotation;
        Vector3 localForce = localShootDirection * new Vector3(0f, 0f, force);
        lineBehaviour.UpdateWithForce(localForce);
    }

    private void UpdateShootInput()
    {
        if (loadingShot)
        {
            // Verifica se il tocco è stato rilasciato
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                swipeEndPos = Input.GetTouch(0).position;
                if (Vector3.Distance(swipeEndPos, swipeStartPos) > swipeThreshold)
                {
                    ShootObject();
                }
                loadingShot = false;
                return;
            }

            var loadingShotTime = Time.time - loadingShotStartTime;
            if (loadingShotTime >= shootAngleOverTime.keys[shootAngleOverTime.length - 1].time)
            {
                shootAngle = -shootAngleOverTime.Evaluate(0f);
                loadingShot = false;
            }

            shootAngle = -shootAngleOverTime.Evaluate(loadingShotTime);

            return;
        }

        // Verifica se il tocco è attivo
        if ((Input.touchCount>0) && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary))
        {
            loadingShot = true;
            loadingShotStartTime = Time.time;
        }

        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            swipeStartPos = Input.GetTouch(0).position;
        }
    }

    void RotatePivot(float rotDirection)
    {
        pivotTransform.rotation *= Quaternion.Euler(0.0f, rotDirection * 1.5f, 0.0f);
    }

    void ShootObject()
    {
        List<GameObject> bulletList = towerManager.GetProjectilesForTower(Menu.currentTowerIndex);
        if (currentBulletIndex >= bulletList.Count)
        {
            return;
        }
        var bulletPrefab = bulletList[currentBulletIndex];
        var shootDirection = Quaternion.AngleAxis(shootAngle, transform.right)* transform.forward;
        var instance = Instantiate(bulletPrefab, transform.position, transform.rotation);
        var block = instance.GetComponent<Block>();       
        block.Shoot(shootDirection * force);
        currentBulletIndex++;
        onShoot.Invoke();
        var remainingItems = Math.Max(0, bulletList.Count - currentBulletIndex);
        remainingText.text = "Remaining Bullet: " + remainingItems.ToString();
    }

    public void TogglePressedRight(bool value)
    {
        isPressedRight = value;
    }

    public void TogglePressedLeft(bool value)
    {
        isPressedLeft = value;
    }
 
}

