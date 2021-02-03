using UnityEngine;

[CreateAssetMenu(fileName = "New Build Blueprint", menuName = "Inventory/Build Blueprint")]
public class BuildBlueprint : ScriptableObject
{
    public GameObject prefab;

    public int sizeX = 1;
    public int sizeY = 1;

    public bool mustBuildOnGround;
    [HideInInspector]public int requiredFloorLevel;

    public void Build()
    {
        //build
    }
}
