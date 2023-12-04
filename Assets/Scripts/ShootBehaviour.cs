using UnityEngine;
using UnityEngine.Events;

public class ShootBehaviour : MonoBehaviour
{
    //[SerializeField]
    //private GameObject projectilePrefab;

    //[SerializeField]
    //public float force = 100.0f;

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
    //private int currentBulletIndex = 0;
    private Quaternion cannonAdditionalRotation;

    [SerializeField] private float force = 1000.0f; // Forza di lancio
    [SerializeField] private float swipeThreshold = 10.0f; // La soglia per considerare uno swipe
    private Vector3 swipeStartPos;
    private Vector3 swipeEndPos;
    [SerializeField] private BulletListAsset bulletListAsset = null;
    

    void Start()
    {
        shootAngle = -shootAngleOverTime.Evaluate(0f);
        cannonAdditionalRotation = Quaternion.AngleAxis(cannonAdditionalRotationAngle, Vector3.right);
    }

    void Update()
    {
        float yDirection = Input.GetAxis("Horizontal");
        RotatePivot(yDirection);

        UpdateShootInput();

        lineBehaviour.SetActive(true);
        UpdateProjectileDirection();
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
        //var shootDirection = Quaternion.AngleAxis(shootAngle, transform.right) * transform.forward;
        //GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, transform.rotation);
        //Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
        //rb.AddForce(shootDirection * force);
        var bulletPrefab = bulletListAsset.Prefabs[0];
        var shootDirection = Quaternion.AngleAxis(shootAngle, transform.right)* transform.forward;
        var instance = Instantiate(bulletPrefab, transform.position, transform.rotation);
        var block = instance.GetComponent<Block>();        
        block.Shoot(shootDirection * force);
        onShoot.Invoke();
    }
}

