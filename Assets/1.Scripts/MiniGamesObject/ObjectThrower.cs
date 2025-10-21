using UnityEngine;
using System.Collections.Generic;
public class ObjectThrower<T> : MonoBehaviour, IPositionTracker where T : Throwable
{
    [Header("ObjectThrower Generics")]
    public bool RotateThrower = true;
    public float shootingLatch = 0.2f;
    public float shootingForce = 10f;
    public float aimLRLengthMul = 5f;
    public Material aimLRMat;
    public GameObject prefab_Bullet;
    public Transform BulletSpawnPoint;
    protected T inst_Bullet;
    protected Vector2 dir;
    LineRenderer aimLR;
    List<T> thrownObjects = new List<T>();
    float lastShootTime = -1f;
    public Vector2 position
    {
        get { return new Vector2(transform.position.x, transform.position.y); }
    }
    public bool IsAiming
    {
        get { return (aimLR != null) && (aimLR.enabled); }
    }
    public void OnPositionChanged(Vector2 iVec2)
    {
        if (!IsAiming)
            return;
        dir = iVec2 - position;
        dir = dir.normalized;
        Aim();
    }

    public void OnStartTracking(Vector2 iVec2)
    {
        if (IsAiming)
            return;
        if (lastShootTime > 0)
            if ((Time.time - lastShootTime) < shootingLatch)
                return;

        InstantiateBullet();

        dir = iVec2 - position;
        dir = dir.normalized;
        StartAim();
    }

    public void OnStopTracking(Vector2 iVec2)
    {
        // Can occur if finger touch from previous game is released in ObjectThrower game
        if (!IsAiming)
            return;

        Aim();
        Shoot();
        dir = Vector2.zero;
    }

    protected virtual void OnPostShoot()
    {

    }

    public void Shoot()
    {
        aimLR.enabled = false;
        inst_Bullet.RB2D.AddForce(shootingForce * dir, ForceMode2D.Impulse);

        OnPostShoot();

        lastShootTime = Time.time;
        inst_Bullet = null;
    }

    public void StartAim()
    {
        if (aimLR == null)
        {
            BuildLR();
        }
        aimLR.enabled = true;
        aimLR.SetPosition(0, transform.position);
        Aim();
    }

    void Aim()
    {
        Vector3 aimSpot = transform.position + new Vector3(dir.x * shootingForce, dir.y * shootingForce, 0f);
        aimLR.SetPosition(1, aimSpot);
        if (RotateThrower)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, transform.forward);
        }
        inst_Bullet.transform.position = BulletSpawnPoint.position;   
    }

    void BuildLR()
    {
        GameObject newGO = new GameObject();
        newGO.name = "AimRenderer";
        newGO.transform.parent = transform;
        newGO.transform.localPosition = Vector3.zero;

        aimLR = newGO.AddComponent<LineRenderer>();
        aimLR.material = aimLRMat;
        aimLR.positionCount = 2;
        aimLR.startWidth = 0.025f;
        aimLR.endWidth = 0.025f;
        aimLR.enabled = false;

    }

    public virtual void InstantiateBullet()
    {
        if (inst_Bullet != null)
        {
            Destroy(inst_Bullet);
        }

        GameObject newGO = Instantiate(prefab_Bullet);
        inst_Bullet = newGO.GetComponent<T>();
        inst_Bullet.transform.parent = transform.parent;
        if (BulletSpawnPoint == null)
            inst_Bullet.transform.position = transform.position;
        else
            inst_Bullet.transform.position = BulletSpawnPoint.position;
        thrownObjects.Add(inst_Bullet);
    }

    public void ClearThrownObjects()
    {
        foreach (T o in thrownObjects)
        {
            if(o != null)
                Destroy(o.gameObject);
        }
        thrownObjects = new List<T>();
    }

    void OnDestroy()
    {
        ClearThrownObjects();
    }
}
