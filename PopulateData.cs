/**
        PopulateData.cs
        Purpose: Scriptable object that have all the data needed
        to crate a planet.
        Require: GenerationData.cs, ColorHeight.cs, PopulateData.cs.

        @author Mikel Jauregui
        @version 1.1.0 14/07/2018 
*/

using UnityEngine;

namespace Generation
{
    [CreateAssetMenu]
    public class PopulateData : ScriptableObject
    {
        // To generate all random numbers.
        private System.Random rng;

        [Tooltip("List of models.")]
        public GameObject[] models;
        [Tooltip("Noise.")]
        public GenerationData noise;
        [Tooltip("Minimum height to get the height.")]
        public int minHeight;
        [Tooltip("Maximum height to get the height.")]
        public int maxHeight;
        [Tooltip("Numbers of divisions.")]
        public int divisions;
        [Range(0,1)]
        [Tooltip("Probability to generatethe model.")]
        public float probability;

        // Offset for the noise.
        private Vector3 offset;

        /// <summary>
        /// Set the seed random number generator and the offset.
        /// </summary>
        /// <param name="seed"> Seed of the planet.</param>
        /// <param name="population"> List of population that want to add the seed.</param>
        static public void SetSeed(int seed, PopulateData[] population)
        {
            foreach (var item in population)
            {
                item.rng = new System.Random(123);
                Vector3 offset = new Vector3(item.rng.Next(-10000, 10000), item.rng.Next(-10000, 10000), item.rng.Next(-10000, 10000));
            }
        }
        /// <summary>
        /// Check if it can be instantiate.
        /// </summary>
        /// <param name="position"> Position to check. </param>
        /// <returns> Return true if it can be instantiate. </returns>
        public bool InstantiateAtPosition(Vector3 position)
        {
            // Get the height at the position.
            float height = noise.CalculateNoise(position + offset);
            // Check if in range.
            if(height > minHeight && height < maxHeight)
                return true;
            return false;
        }
        /// <summary>
        /// Get a random model from the list.
        /// </summary>
        /// <returns> Return a model.</returns>
        public GameObject GetRandomModel()
        {
            // Get random model.
            return models[rng.Next(0, models.Length)];
        }
        /// <summary>
        /// Get the height at that position.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>Return the height.</returns>
        public float GetHeightAtPosition(Vector3 position)
        {
            // Get noise at position.
            return noise.CalculateNoise(position + offset);
        }
    }
}
