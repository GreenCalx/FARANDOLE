using UnityEngine;

public interface IDogFamily: IMiniGameFamily
{
    public EMiniGameFamily AssociatedTag() { return EMiniGameFamily.DOG; }

}