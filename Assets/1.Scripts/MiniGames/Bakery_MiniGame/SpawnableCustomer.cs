using UnityEngine;

public class SpawnableCustomer : Customer, ISpawnable
{
    public void OnCommandSatisfied()
    {
        GameObject.Destroy(handle_desiredItemDisplay.gameObject);
    }
}