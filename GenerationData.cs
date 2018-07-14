/**
        GenerationData.cs
        Purpose: Scriptable object that stores the
        data and some functions for the terrain generation.
        Require: No other files required.

        @author Mikel Jauregui
        @version 1.1.0 14/07/2018 
*/

using UnityEngine;

namespace Generation
{
    [CreateAssetMenu()]
    public class GenerationData : ScriptableObject
    {
        [Range(1, 10)]
        [Tooltip("Amounts of calls.")]
        public int octaves = 8;
        [Range(0.1f, 1.0f)]
        [Tooltip("Amplitud variation through octaves.")]
        public float persistance = 0.5f;
        [Range(1.0f, 2f)]
        [Tooltip("Frequency variation through octaves.")]
        public float lacunarity = 2f;
        [Range(0.1f, 500)]
        [Tooltip("General frequency division.")]
        public float scale = 1;
        [Range(0f,1f)]
        [Tooltip("Minimum perlin value that return a value.")]
        public float minimum_height = 0.5f;
        [Range(1f,500f)]
        [Tooltip("Height that will be multiply to the 0 to 1 value.")]
        public int height_multiplier = 1;
        [Tooltip("True to invert the values sign.")]
        public bool invert = false;

        /// <summary>
        /// Genrate 3D perlin noise value.
        /// </summary>
        /// <param name="x"> x axis. </param>
        /// <param name="y"> y axis. </param>
        /// <param name="z">z axis. </param>
        /// <returns> Return a float between 0 and 1. </returns>
        static private float PerlinNoise3D(float x, float y, float z)
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

        /// <summary>
        /// Calculate the noise at a specific position.
        /// </summary>
        /// <param name="position"> Position. </param>
        /// <returns> Returns the height at that position.</returns>
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
            // Make output to stay between 0 and 1.
            output /= octaves;

            // if output is less than minimum_height then set to 0.
            output = output > minimum_height ? Mathf.InverseLerp(0, 1f - minimum_height, output - minimum_height) : 0;
            output *= height_multiplier;
            if (invert)
                output *= -1;
            return output;
        }
    }
}
