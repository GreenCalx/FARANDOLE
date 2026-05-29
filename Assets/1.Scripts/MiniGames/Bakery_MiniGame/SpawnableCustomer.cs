using UnityEngine;

public class SpawnableCustomer : Customer, ISpawnable
{
    public Transform handle_desiredItemDisplay;

    public void OnCommandSatisfied()
    {
        GameObject.Destroy(handle_desiredItemDisplay.gameObject);
    }
}