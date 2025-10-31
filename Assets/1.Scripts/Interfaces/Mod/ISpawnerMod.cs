using UnityEngine;

public interface ISpawnerMod<T> : IMiniGameMod where T: ISpawnable
{
    public EMiniGameTags AssociatedTag() { return EMiniGameTags.SPAWNER; }
    public T Spawn(GameObject iPrefab);
}