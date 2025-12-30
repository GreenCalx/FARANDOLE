using UnityEngine;

public interface IRegularFamily : IMiniGameFamily
{
    public EMiniGameFamily AssociatedTag() { return EMiniGameFamily.REGULAR; }

}