using UnityEngine;
using Generation;

[SerializeField]
public class Main : MonoBehaviour
{
    public PlanetData[] planets;
    public RuntimeAnimatorController controller;

    public void Start()
    {
        foreach (var item in planets)
            Planet.AddPlanetToQueue(item.newName, item.position, item.seed,
                item.generateRandomSeed, item.terrainStyle, item.generationData, item.ColorPerLayer,
                item.material, item.radius, item.subdivisions, item.chunckSubdivisions, controller);

        Planet.StartDataQueue();
    }
    private void Update()
    {
        Planet.InstantiateIntoWorld();
    }
}
