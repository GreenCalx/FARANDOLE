using UnityEngine;

public class DartThrower : ObjectThrower<Dart>
{
    public float torqueForce = 10f;
    protected override void OnPostShoot()
    {
        inst_Bullet.RB2D.AddTorque(torqueForce);
    }
}
