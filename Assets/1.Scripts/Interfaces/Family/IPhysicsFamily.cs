using UnityEngine;

public interface IPhysicsFamily : IMiniGameFamily
{
    public EMiniGameFamily AssociatedTag() { return EMiniGameFamily.PHYSICS; }

}