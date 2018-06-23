using UnityEngine;
using Generation;

public class Main : MonoBehaviour
{
    public RuntimeAnimatorController controller;
    public string name;
    public Vector3 position;
    public int seed;
    public GenerationData[] general;
    public Material ma;
    public int radius;
    public int sphereSubdivision;
    public int chunckSubdivision;

    public void Start()
    {
        seed = Random.Range(0, int.MaxValue);
        Planet.AddPlanetToQueue(name, position, seed, general, ma, radius, sphereSubdivision, chunckSubdivision, controller);
        Planet.StartDataQueue();
    }
    private void Update()
    {
        Planet.StartGeneratingMeshDataQueue();
        Planet.InstantiateIntoWorld();
    }
}
