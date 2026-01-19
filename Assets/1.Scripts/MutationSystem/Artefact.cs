using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Artefact
{
    public ArtefactSO data;
    public List<IMiniGameMod> mods;

    public Artefact(ArtefactSO iDef)
    {
        data = iDef;
        mods = new List<IMiniGameMod>();
    }
}