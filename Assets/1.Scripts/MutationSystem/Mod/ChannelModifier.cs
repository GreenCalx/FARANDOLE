using UnityEngine;

public class ChannelModifiier : IMiniGameMod
{
    public EMiniGameMods AssociatedTag()
        => EMiniGameMods.NONE;
    public int AlphaChannel = 0;
    public int BetaChannel =  0;
    public void Apply(MiniGameLoopSocket socket)
    {
        socket.alpha_channel += AlphaChannel;
        socket.beta_channel += BetaChannel;
    }
}