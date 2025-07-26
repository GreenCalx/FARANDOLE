using UnityEngine;

public class ObjectThrower<T> : MonoBehaviour, IPositionTracker where T : Throwable
{
    [Header("ObjectThrower Generics")]
    public float shootingForce = 10f;
    public float aimLRLengthMul = 5f;
    public Material aimLRMat;
    public GameObject prefab_Bullet;
    protected T inst_Bullet;
    protected Vector2 dir;
    LineRenderer aimLR;
    public Vector2 position
    {
        get { return new Vector2(transform.position.x, transform.position.y); }
    }
    public bool IsAiming
    {
        get { return (aimLR!=null)&&(aimLR.enabled); }
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

    public void Shoot()
    {
        aimLR.enabled = false;
        inst_Bullet.RB2D.AddForce(shootingForce * dir, ForceMode2D.Impulse);
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
        aimLR.SetPosition(1, transform.position + new Vector3(dir.x* shootingForce, dir.y* shootingForce, 0f));
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
        aimLR.startWidth = 0.05f;
        aimLR.endWidth = 0.05f;
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
        inst_Bullet.transform.position = transform.position;
    }
}
