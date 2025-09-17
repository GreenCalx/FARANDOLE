using UnityEngine;
using UnityEngine.UI;

public class UIImageColorToShader : MonoBehaviour
{
    public Image target;
    public string shaderParm = "_Color";
    public bool continuousRefresh = true;
    Color currentColor;
    Color initColor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Likely to be default
        if (target == null)
            target = GetComponent<Image>();
        if (string.IsNullOrEmpty(shaderParm))
            shaderParm = "_Color";

        initColor = target.color;
        currentColor = initColor;
        target.material.SetColor(shaderParm, initColor);
    }

    void OnDisable()
    {
        if (target == null)
            return;
        target.material.SetColor(shaderParm, initColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (continuousRefresh)
        {
            if (!currentColor.Equals(target.color))
            {
                currentColor = target.color;
                target.material.SetColor(shaderParm, currentColor);
            }
        }
    }

    void UpdateMaterial()
    {
        
    }
}
