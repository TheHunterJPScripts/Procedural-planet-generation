using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    [CreateAssetMenu()]
    [SerializeField]
    public class GenerationData : ScriptableObject
    {
        static public float PerlinNoise3D(float x, float y, float z)
        {

            float ab = Mathf.PerlinNoise(x, y);
            float bc = Mathf.PerlinNoise(y, z);
            float ac = Mathf.PerlinNoise(x, z);

            float ba = Mathf.PerlinNoise(y, x);
            float cb = Mathf.PerlinNoise(z, y);
            float ca = Mathf.PerlinNoise(z, x);

            float abc = ab + bc + ac + ba + cb + ca;
            return abc / 6f;
        }

        [Range(1, 20)]
        public int octaves = 8;
        [Range(0.1f, 1.0f)]
        public float persistance = 0.5f;
        [Range(1.0f, 2f)]
        public float lacunarity = 2f;
        [Range(0.1f, 500)]
        public float scale = 1;
        public float minimum_height = 0.5f;
        public int height_multiplier = 1;
        public bool invert = false;

        public GenerationData(int octaves = 8, float persistance = 0.5f, float lacunarity = 2.0f, float scale = 1.0f, float minimum_height = 0.0f, int height_multiplier = 1, bool invert = false)
        {
            this.octaves = octaves;
            this.persistance = persistance;
            this.lacunarity = lacunarity;
            this.scale = scale;
            this.minimum_height = minimum_height;
            this.height_multiplier = height_multiplier;
            this.invert = invert;
        }
        public float CalculateNoise(Vector3 position)
        {
            float x = position.x;
            float y = position.y;
            float z = position.z;

            float output = 0;
            float frequency = 1;
            float amplitude = 1;

            for (int i = 0; i < octaves; i++)
            {
                output += PerlinNoise3D(x * frequency / scale, y * frequency / scale, z * frequency / scale) * amplitude;
                amplitude *= persistance;
                frequency *= lacunarity;
            }
            output /= octaves;
            output = output > minimum_height ? output - minimum_height : 0;
            output *= height_multiplier;
            if (invert)
                output *= -1;
            return output;
        }
    }
}
