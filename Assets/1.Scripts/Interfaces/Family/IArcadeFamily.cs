using UnityEngine;

public interface IArcadeFamily : IMiniGameFamily
{
    public EMiniGameFamily AssociatedTag() { return EMiniGameFamily.ARCADE; }

}