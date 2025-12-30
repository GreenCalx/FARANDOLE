using UnityEngine;

public interface IChessFamily : IMiniGameFamily
{
    public EMiniGameFamily AssociatedTag() { return EMiniGameFamily.CHESS; }

}