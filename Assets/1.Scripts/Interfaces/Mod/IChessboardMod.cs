using UnityEngine;

public interface IChessboardMod : IMiniGameMod
{
    public EMiniGameMods AssociatedTag() { return EMiniGameMods.CHESSBOARD; }

}