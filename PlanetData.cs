using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Generation;

namespace Generation
{
    [CreateAssetMenu()]
    [Serializable]
    public class PlanetData : ScriptableObject
    {
        public string newName;
        public int seed;
        public bool generateRandomSeed;
        public Style terrainStyle;
        [Space(5)]
        public Vector3 position;
        public Material material;
        [Space(5)]
        public float radius;
        public int subdivisions;
        public int chunckSubdivisions;
        [Space(5)]
        public GenerationData[] generationData;
        public ColorHeight[] ColorPerLayer;
    }
    public enum Style { LowPoly, Terrace };
}