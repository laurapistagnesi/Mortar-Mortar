﻿using System;
using Supyrb;
using UnityEngine;
using UnityEngine.Events;

public class ShootBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameData gameData = null;
    
    [SerializeField]
    private BulletListAsset bulletListAsset = null;

    [SerializeField]
    private KeyCode fire = KeyCode.Space;
    
    [SerializeField]
    public float force = 100.0f;

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
    private Vector3 forceVector;
    private float shootAngle;
    private bool loadingShot;
    private float loadingShotStartTime;
    private int currentBulletIndex = 0;
    private Quaternion cannonAdditionalRotation;
    
    private MortaShootSignal mortaShootSignal;
    private AllBulletsShotSignal allBulletsShotSignal;
    private RemainingItemsSignal remainingItemsSignal;
    private LoadLevelSignal loadLevelSignal;
    private RestartLevelSignal restartLevelSignal;
    private ToMenuSignal toMenuSignal;

    private void Start()
    {
        shootAngle = - shootAngleOverTime.FirstValue();
        cannonAdditionalRotation = Quaternion.AngleAxis(cannonAdditionalRotationAngle, Vector3.right);
        Signals.Get(out mortaShootSignal);
        Signals.Get(out allBulletsShotSignal);
        Signals.Get(out remainingItemsSignal);
        Signals.Get(out loadLevelSignal);
        Signals.Get(out restartLevelSignal);
        Signals.Get(out toMenuSignal);

        loadLevelSignal.AddListener(OnLoadLevel);
        restartLevelSignal.AddListener(OnRestartLevel);
        toMenuSignal.AddListener(OnRestartLevel);
    }

    private void OnDestroy()
    {
        loadLevelSignal.RemoveListener(OnLoadLevel);
        restartLevelSignal.RemoveListener(OnRestartLevel);
        toMenuSignal.RemoveListener(OnRestartLevel);
    }

    void Update(){
        if (gameData.CurrentState != GameState.Game)
        {
            lineBehaviour.SetActive(false);
            return;
        }
        float yDirection = Input.GetAxis("Horizontal");
        RotatePivot(yDirection);

        UpdateShootInput();
        
        lineBehaviour.SetActive(true);
        UpdateProjectileDirection();

    }

    private void UpdateProjectileDirection()
    {
        var localShootDirection = Quaternion.AngleAxis(shootAngle, Vector3.right);
        cannonTubeTransform.localRotation = localShootDirection  * cannonAdditionalRotation;
        Vector3 localForce = localShootDirection * new Vector3(0f, 0f, force);
        lineBehaviour.UpdateWithForce(localForce);        
    }

    private void UpdateShootInput()
    {
        if (loadingShot)
        {
            if (Input.GetKeyUp(fire))
            {
                ShootObject();
                loadingShot = false;
                return;
            }

            var loadingShotTime = Time.time - loadingShotStartTime;
            if (loadingShotTime >= shootAngleOverTime.Duration())
            {
                shootAngle = - shootAngleOverTime.FirstValue();
                loadingShot = false;
            }

            shootAngle = - shootAngleOverTime.Evaluate(loadingShotTime);

            return;
        }

        if (Input.GetKey(fire))
        {
            loadingShot = true;
            loadingShotStartTime = Time.time;
        }
    }

    void RotatePivot(float rotDirection){
        pivotTransform.rotation *= Quaternion.Euler(0.0f,rotDirection*1.5f,0.0f);        
    }
    
    void ShootObject()
    {
        if (currentBulletIndex >= bulletListAsset.Prefabs.Length)
        {
            return;
        }

        var bulletPrefab = bulletListAsset.Prefabs[currentBulletIndex];
        var shootDirection = Quaternion.AngleAxis(shootAngle, transform.right)* transform.forward;
        var instance = Instantiate(bulletPrefab, transform.position, transform.rotation);
        var block = instance.GetComponent<Block>();        
        block.Shoot(shootDirection * force);
        mortaShootSignal.Dispatch();
        currentBulletIndex++;
        onShoot.Invoke();

        var remainingItems = Math.Max(0, bulletListAsset.Prefabs.Length - currentBulletIndex);
        remainingItemsSignal.Dispatch(remainingItems);
        if (remainingItems == 0)
        {
            allBulletsShotSignal.Dispatch();
        }
    }
    
    private void OnLoadLevel(LevelAsset levelAsset)
    {
        bulletListAsset = levelAsset.BulletList;
        currentBulletIndex = 0;
        remainingItemsSignal.Dispatch(bulletListAsset.Prefabs.Length);
    }
    
    private void OnRestartLevel()
    {
        currentBulletIndex = 0;
        remainingItemsSignal.Dispatch(bulletListAsset.Prefabs.Length);
    }
}
