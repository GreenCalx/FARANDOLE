using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;

public class ShaderGraphBaker : EditorWindow
{
    private Material shaderGraphMaterial;
    private int resolution = 512;
    private Color background = Color.black;
    private string fileName = "BakedShaderGraph";

    [MenuItem("Tools/URP ShaderGraph Baker")]
    public static void OpenWindow()
    {
        GetWindow<ShaderGraphBaker>("URP ShaderGraph Baker");
    }

    private void OnGUI()
    {
        GUILayout.Label("🎨 URP ShaderGraph Texture Baker", EditorStyles.boldLabel);
        shaderGraphMaterial = (Material)EditorGUILayout.ObjectField("ShaderGraph Material", shaderGraphMaterial, typeof(Material), false);
        resolution = EditorGUILayout.IntPopup("Resolution", resolution, new[] { "256", "512", "1024", "2048" }, new[] { 256, 512, 1024, 2048 });
        background = EditorGUILayout.ColorField("Background Color", background);
        fileName = EditorGUILayout.TextField("File Name", fileName);
        EditorGUILayout.Space();
        GUI.enabled = shaderGraphMaterial != null;
        if (GUILayout.Button("Bake ShaderGraph to PNG", GUILayout.Height(30)))
        {
            BakeShaderGraphURP();
        }
        GUI.enabled = true;
    }

    private void BakeShaderGraphURP()
    {
        if (shaderGraphMaterial == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a Shader Graph material.", "OK");
            return;
        }

        // Create render camera
        GameObject camObj = new GameObject("TempBakeCam");
        Camera cam = camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 1;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = background;
        cam.cullingMask = ~0;
        cam.transform.position = new Vector3(0, 0, -10);

        // Create a quad with the material
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.GetComponent<Renderer>().sharedMaterial = shaderGraphMaterial;
        quad.transform.position = Vector3.zero;
        quad.transform.rotation = Quaternion.identity;
        quad.transform.localScale = Vector3.one * 2f;

        // Create RenderTexture
        RenderTexture rt = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;

        cam.Render();

        // Read pixels
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        tex.Apply();

        // Save PNG
        string path = EditorUtility.SaveFilePanel("Save Baked Texture", Application.dataPath, $"{fileName}.png", "png");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Debug.Log($"✅ ShaderGraph baked to: {path}");
            AssetDatabase.Refresh();
        }

        // Cleanup
        RenderTexture.active = null;
        DestroyImmediate(quad);
        DestroyImmediate(camObj);
        DestroyImmediate(rt);
        DestroyImmediate(tex);
    }
}
