/**
        PlanetData.cs
        Purpose: Scriptable object that have all the data needed
        to crate a planet.
        Require: GenerationData.cs, ColorHeight.cs, PopulateData.cs.

        @author Mikel Jauregui
        @version 1.1.0 14/07/2018 
*/

using UnityEngine;

namespace Generation
{
    [CreateAssetMenu()]
    public class PlanetData : ScriptableObject
    {
        [Tooltip("Name for the Empty Object that will hold the terrains.")]
        public string newName;
        [Tooltip("Seed for the planet.")]
        public int seed;
        [Tooltip("If true it will ignore the seed and generate a new random one.")]
        public bool generateRandomSeed;
        [Tooltip("Types of terrain generation.")]
        public Style terrainStyle;
        [Tooltip("Material for the terrain(to be able to apply the color per layer the material need to be vertex friendly).")]
        public Material material;
        [Tooltip("Material for the sea.")]
        public Material seaMaterial;
        [Space(5)]
        [Tooltip("Position of the center of the planet.")]
        public Vector3 position;
        [Tooltip("Radius of the planet.")]
        public float radius;
        [Tooltip("Level of the sea.")]
        public float seaLevel;
        [Tooltip("Amount of times the  icosphere is divided.")]
        public int subdivisions;
        [Tooltip("Amount of times each icosphere division is divided.")]
        public int chunckSubdivisions;
        [Space(5)]
        [Tooltip("Noise to modify the terrain shape.")]
        public GenerationData[] generationData;
        [Tooltip("Colors for the terrain.")]
        public ColorHeight[] ColorPerLayer;
        [Tooltip("Data to populate the terrain.")]
        public PopulateData[] population;
    }
    /// <summary>
    /// Types of terrains.
    /// </summary>
    public enum Style { LowPoly, Terrace };
}