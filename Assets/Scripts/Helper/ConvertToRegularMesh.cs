using UnityEngine;

public class ConvertToRegularMesh : MonoBehaviour
{
    [ContextMenu("Convert to regular mesh")]
    void Convert()
    {
        SkinnedMeshRenderer skinRend = GetComponent<SkinnedMeshRenderer>();
        MeshRenderer rend = gameObject.AddComponent<MeshRenderer>();
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();

        filter.sharedMesh = skinRend.sharedMesh;
        rend.sharedMaterials = skinRend.sharedMaterials;

        DestroyImmediate(skinRend);
        DestroyImmediate(this);
    }
}
