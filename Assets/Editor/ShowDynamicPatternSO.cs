#if !UNITY_ANDROID
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DynamicPatternSO))]
public class ShowDynamicPatternSO : Editor
{
    private PreviewRenderUtility previewUtility;
    private Mesh planeMesh;
    private Material previewMaterial;
    private DynamicPatternSO currentData;

    private void OnEnable()
    {
        CleanupPreview();
        SetupPreview();
    }

    private void OnDisable()
    {
        CleanupPreview();
    }

    private void SetupPreview()
    {
        currentData = (DynamicPatternSO)target;

        // Create the preview utility (isolated camera + light)
        previewUtility = new PreviewRenderUtility();
        previewUtility.cameraFieldOfView = 30f;
        previewUtility.camera.transform.position = new Vector3(0, 0, -2f);
        previewUtility.camera.transform.LookAt(Vector3.zero);
        previewUtility.lights[0].intensity = 1.1f;
        previewUtility.lights[0].transform.rotation = Quaternion.Euler(40f, 40f, 0);

        // Create a simple plane mesh
        planeMesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
        if (planeMesh == null)
            planeMesh = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshFilter>().sharedMesh;

        // Prepare preview material
        previewMaterial = new Material(Shader.Find("XL/MobileDynamicPattern"));
        UpdateMaterialFromData(previewMaterial, currentData);
    }

    private void CleanupPreview()
    {
        if (previewUtility != null)
        {
            previewUtility.Cleanup();
            previewUtility = null;
        }
        if (previewMaterial != null)
        {
            DestroyImmediate(previewMaterial);
            previewMaterial = null;
        }
    }

    public override bool HasPreviewGUI() => true;

    public override void OnInspectorGUI()
    {
        // Draw normal inspector
        base.OnInspectorGUI();

        // Live update the preview material
        if (previewMaterial != null)
            UpdateMaterialFromData(previewMaterial, (DynamicPatternSO)target);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Material Preview", EditorStyles.boldLabel);

        Rect previewRect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(false));
        DrawPreview(previewRect);
    }

    private void DrawPreview(Rect rect)
    {
        if (previewUtility == null || previewMaterial == null)
            return;

        previewUtility.BeginPreview(rect, GUIStyle.none);

        // Draw plane with current material
        var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 0), Vector3.one);
        previewUtility.DrawMesh(planeMesh, matrix, previewMaterial, 0);

        // Render the preview camera
        previewUtility.camera.Render();

        // Display the result in the Inspector
        Texture result = previewUtility.EndPreview();
        GUI.DrawTexture(rect, result, ScaleMode.StretchToFill, false);
    }

    private void UpdateMaterialFromData(Material mat, DynamicPatternSO data)
    {
        if (data == null || mat == null)
            return;

        mat.SetInt("_PatternA", (int)data.pattern);
        mat.SetFloat("_Angle", data.angle);
        mat.SetFloat("_BoxSize", data.boxSize);
        mat.SetFloat("_Tiling", data.tiling);
        mat.SetVector("_PatternOffset", new Vector4 ( data.offset.x, data.offset.y, 0, 0 ));
        mat.SetVector("_TruchetRotations", new Vector4 (
            data.truchetAngles.x, data.truchetAngles.y, data.truchetAngles.z, data.truchetAngles.w
        ));
    }
}
#endif