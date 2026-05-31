using UnityEngine;

public class SpawnableCustomer : Customer, ISpawnable
{
    // handle_desiredItemDisplay is inherited from Customer; re-declaring it here made
    // Unity serialize the same field name twice (type-tree error on every deserialize).

    public void OnCommandSatisfied()
    {
        GameObject.Destroy(handle_desiredItemDisplay.gameObject);
    }
}