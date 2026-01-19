using UnityEngine;

public class ChannelInverterModifiier : IMiniGameMod
{
    public EMiniGameMods AssociatedTag()
        => EMiniGameMods.NONE;
    public bool invert = true;
    public void Apply(MiniGameLoopSocket socket)
    {
        var temp = socket.alpha_channel;
        socket.alpha_channel = socket.beta_channel;
        socket.beta_channel = temp;
    }
}