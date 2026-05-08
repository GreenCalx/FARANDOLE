using UnityEngine;

public class Propeller : MonoBehaviour
{
    [Header("Refs")]
    public PropellerRotater propellerRotater;

    public float AngularSpeed => propellerRotater.AngularSpeed;

    public void Init()
    {
        propellerRotater.Init();
    }
}
