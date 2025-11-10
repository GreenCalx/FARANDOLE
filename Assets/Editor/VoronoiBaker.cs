using UnityEngine;
using UnityEditor;

public class VoronoiBaker : EditorWindow
{
    Material voronoiMat;
    int textureSize = 512;

    [MenuItem("Tools/Bake Voronoi Texture")]
    public static void ShowWindow()
    {
        GetWindow<VoronoiBaker>("Voronoi Baker");
    }

    void OnGUI()
    {
        GUILayout.Label("Bake Voronoi ShaderGraph", EditorStyles.boldLabel);
        voronoiMat = (Material)EditorGUILayout.ObjectField("Voronoi Material", voronoiMat, typeof(Material), false);
        textureSize = EditorGUILayout.IntField("Texture Size", textureSize);

        if (GUILayout.Button("Bake to PNG"))
        {
            if (voronoiMat == null)
            {
                Debug.LogError("Assign a Voronoi material first!");
                return;
            }

            BakeVoronoi();
        }
    }

    void BakeVoronoi()
    {
        RenderTexture rt = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(null, rt, voronoiMat);

        Texture2D tex = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        string path = EditorUtility.SaveFilePanelInProject("Save Voronoi Texture", "VoronoiBaked", "png", "Choose save location for baked Voronoi texture");
        if (!string.IsNullOrEmpty(path))
        {
            byte[] png = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, png);
            AssetDatabase.Refresh();
            Debug.Log($"✅ Baked Voronoi texture saved to: {path}");
        }

        rt.Release();
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(tex);
    }
}
