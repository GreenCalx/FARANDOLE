using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;

public class TitleAnim : MonoBehaviour
{
    float initCellScale,
            initTimeScale,
            initInnerCellColor,
            initOutCellColor,
            initTint;
    const string sParmCellScale = "_CellScale";
    const string sParmTimeScale = "_TimeScale";
    const string sParmInnerCellColor = "_InnerCellColor";
    const string sParmOutCellColor = "_OutCellColor";
    const string sParmTint = "_CellTint";
    public Material titleMat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initCellScale = titleMat.GetFloat(sParmCellScale);
        initTimeScale = titleMat.GetFloat(sParmTimeScale);
        initInnerCellColor = titleMat.GetFloat(sParmInnerCellColor);
        initOutCellColor = titleMat.GetFloat(sParmOutCellColor);
        initTint = titleMat.GetFloat(sParmTint);
    }

    public void ExitAnim()
    {
        // TODO lerp cell scale

        //titleMat.SetFloat();
        
        // TODO lerp tint to full black
    }
}
