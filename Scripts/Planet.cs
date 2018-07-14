/**
        Planet.cs
        Purpose: Generate Planets into the scene.
        Require: Polygon.cs, GenerationData.cs,
        ColorHeight.cs, PopulationData.cs.

        @author Mikel Jauregui
        @version 1.1.0 14/07/18 
*/

using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace Generation
{
    public class Planet : MonoBehaviour
    {
        // List where we store all the planets.
        static private List<Planet> planetList = new List<Planet>();
        // Queue where we add the polygons that need to be instantiate.
        static private Queue<Polygon> polygonsToInstantiate = new Queue<Polygon>();
        // List of polygons instantiated.
        static List<Polygon> distanceList = new List<Polygon>();
        // True if the thread that generate 
        // the data for the planet has started.
        static private bool isThreadBeenStarted = false;
        // True if the thread that generate the
        // data for the planet has ended.
        static private bool isThreadDone = false;
        // True if the game want to quit.
        static private bool gameQuiting = false;
        // The terrain has been instantiate.
        static private bool terrainInstantiated = false;

        // Stores a reference to its own 
        // to pass it to the polygons.
        private Planet planet;
        // Terrain style for the planet.
        private Style terrainStyle;
        // Array of noises that will 
        // then be used to generate the terrain.
        private GenerationData[] generationData;
        // Array of colors that will then be used
        // to color the terrain.
        private ColorHeight[] colorHeight;
        // Array of offsets that will be apply
        // to the terrain generation noise.
        private Vector3[] offset;
        // Array that stores the data to 
        // generate the population for the planet.
        private PopulateData[] population;
        // Material for the terrain.
        private Material material;
        // Material for the sea.
        private Material seaMaterial;
        // List of polygons that compose the planet.
        private List<Polygon> polygons;
        // List of vertices of the icosphere.
        private List<Vector3> vertices;
        // Center of the planet.
        private Vector3 position;
        // Planet radius.
        private float radius;
        // Sea Level.
        private float seaLevel;
        // Planet seed.
        private int seed;
        // Icosphere subdivisions.
        private int subdivisions;
        // Chunck subdivisions.
        private int chunckSubdivisions;

        /// <summary>
        /// Thread that generate all the data.
        /// </summary>
        static private void DataThreadLoop()
        {
            //Prevent list modification during
            // thread loop.
            lock (planetList)
            {
                foreach (var planet in planetList)
                {
                    if (gameQuiting)
                        return;
                    // Create icosahedron.
                    planet.InitAsIcosahedron();
                    // Subdivide it.
                    planet.Subdivide();
                    // Create noise offset.
                    planet.CalculateOffset();

                    // Generate mesh data.
                    foreach (var poly in planet.polygons)
                    {
                        if (gameQuiting)
                            return;

                        poly.GenerateMesh();
                    }
                }
            }
            isThreadDone = true;
        }
        /// <summary>
        /// Check is a polygon is in range of the viewer.
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        static private bool PolyInRange(Polygon poly, Vector3 position)
        {
            // Normalize to check on the circle unit.
            position = position.normalized;

            // Get the planet data.
            Planet p = poly.GetPlanet();
            int[] i = poly.GetVertices().ToArray();
            Vector3[] v = new Vector3[3] { p.vertices[i[0]], p.vertices[i[1]], p.vertices[i[2]] };
            float d0, d1, d2;
            d0 = Vector3.Distance(position, v[0].normalized);
            d1 = Vector3.Distance(position, v[1].normalized);
            d2 = Vector3.Distance(position, v[2].normalized);
            // True if in range.
            return d0 < Main.G_viewDistance || d1 < Main.G_viewDistance || d2 < Main.G_viewDistance;
        }
        /// <summary>
        /// Add a polygon to the Instantiate queue.
        /// </summary>
        /// <param name="poly"> The polygon that need to be added. </param>
        static public void AddPolygonToQueue(Polygon poly)
        {
            lock (polygonsToInstantiate)
            {
                // Add to end of the queue.
                polygonsToInstantiate.Enqueue(poly);
            }
        }
        /// <summary>
        /// Check if the main thread want to quit.
        /// </summary>
        /// <returns> True if the game is quitting. </returns>
        static public bool ThreadNeedToQuit()
        {
            return gameQuiting;
        }
        /// <summary>
        /// Add a planet to the queue of the ones that need to be generated.
        /// </summary>
        /// <param name="name">Name of the Empty object that will hold the terrain objects</param>
        /// <param name="position">Center of the planet.</param>
        /// <param name="seed">Seed for the planet.</param>
        /// <param name="randomSeed"> True if the seed need to be change by a random new one.</param>
        /// <param name="terrainStyle">Style of the terrain.</param>
        /// <param name="seaLevel">Level of the sea.</param>
        /// <param name="generationData">Noise to generate the terrain.</param>
        /// <param name="colorHeight">Color to add to the layers.</param>
        /// <param name="population">Data to populate the planet.</param>
        /// <param name="material">Material for the terrain.</param>
        /// <param name="seaMaterial">Material for the sea.</param>
        /// <param name="radius">Radius of the planet.</param>
        /// <param name="subdivisions">Icosphere subdivisions.</param>
        /// <param name="chunckSubdivisions">Chunck subdivisions.</param>
        static public void AddPlanetToQueue(string name, Vector3 position, int seed, bool randomSeed, Style terrainStyle, float seaLevel, GenerationData[] generationData, ColorHeight[] colorHeight, PopulateData[] population, Material material, Material seaMaterial, float radius, int subdivisions, int chunckSubdivisions)
        {
            // Instantiate a gameobject that hold planet.
            GameObject obj = new GameObject();
            obj.name = name;
            obj.transform.position = position;
            Planet planet = obj.AddComponent<Planet>();
            planet.planet = planet;
            int new_seed = randomSeed == true ? UnityEngine.Random.Range(0, int.MaxValue) : seed;
            planet.InstantiatePlanetVariables(new_seed, terrainStyle, seaLevel, generationData, colorHeight, population, material, seaMaterial, radius, subdivisions, chunckSubdivisions);
            planet.position = position;

            // Add the planet to the list of planets.
            planetList.Add(planet);
        }
        /// <summary>
        /// Start the Data computations.
        /// </summary>
        static public void StartDataQueue()
        {
            // Prevent to overload of threads. Only
            // one will be running at a time.
            if (isThreadBeenStarted)
                return;

            // Start the thread.
            Thread thread = new Thread(DataThreadLoop);
            thread.Start();
            isThreadBeenStarted = true;
        }
        /// <summary>
        /// Generate the terrain while added to the queue.
        /// </summary>
        static public void InstantiateIntoWorld()
        {
            terrainInstantiated = true;
            // Instantiate the polygons into the scene while been added to the queue.
            while (polygonsToInstantiate.Count != 0)
            {
                Polygon poly = polygonsToInstantiate.Dequeue();
                poly.Instantiate();
                // Add it to the list for later modification.
                distanceList.Add(poly);
            }
        }
        /// <summary>
        /// Hide or show the polygons objects
        /// if they are in range of the viewer.
        /// </summary>
        /// <param name="viewer"> Position tto check if the polygon is in range. </param>
        static public void HideAndShow(Vector3 viewer)
        {
            // Wait to the secondary thread to end for avoid errors.
            if (!isThreadDone)
                return;

            // if distanceList is empty fill it.
            if(!terrainInstantiated)
                while (polygonsToInstantiate.Count != 0)
                    distanceList.Add(polygonsToInstantiate.Dequeue());

            foreach (var item in distanceList)
            {
                // if polygon is in range to the viewer instantiate.
                if(PolyInRange(item, viewer))
                    item.Show();
                // else hide it.
                else
                    item.Hide();
            }
        }
        /// <summary>
        /// Set the vairables to the values.
        /// </summary>
        /// <param name="seed">Seed for the planet.</param>
        /// <param name="randomSeed"> True if the seed need to be change by a random new one.</param>
        /// <param name="terrainStyle">Style of the terrain.</param>
        /// <param name="seaLevel">Level of the sea.</param>
        /// <param name="generationData">Noise to generate the terrain.</param>
        /// <param name="colorHeight">Color to add to the layers.</param>
        /// <param name="population">Data to populate the planet.</param>
        /// <param name="material">Material for the terrain.</param>
        /// <param name="seaMaterial">Material for the sea.</param>
        /// <param name="radius">Radius of the planet.</param>
        /// <param name="subdivisions">Icosphere subdivisions.</param>
        /// <param name="chunckSubdivisions">Chunck subdivisions.</param>
        private void InstantiatePlanetVariables(int seed, Style terrainStyle, float seaLevel, GenerationData[] generationData, ColorHeight[] colorHeight, PopulateData[] population, Material material, Material seaMaterial, float radius, int subdivisions, int chunckSubdivisions)
        {
            this.seed = seed;
            this.terrainStyle = terrainStyle;
            this.radius = radius;
            this.generationData = generationData;
            this.colorHeight = colorHeight;
            this.population = population;
            PopulateData.SetSeed(seed, population);
            this.subdivisions = subdivisions;
            this.seaLevel = seaLevel;
            this.material = material;
            this.seaMaterial = seaMaterial;
            this.chunckSubdivisions = chunckSubdivisions;
        }
        /// <summary>
        /// Generate the basic Icosphere structure.
        /// </summary>
        private void InitAsIcosahedron()
        {
            polygons = new List<Polygon>();
            vertices = new List<Vector3>();

            // An icosahedron has 12 vertices, and
            // since it's completely symmetrical the
            // formula for calculating them is kind of
            // symmetrical too:

            float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

            vertices.Add(position + new Vector3(-1, t, 0).normalized * this.radius);
            vertices.Add(position + new Vector3(1, t, 0).normalized * this.radius);
            vertices.Add(position + new Vector3(-1, -t, 0).normalized * this.radius);
            vertices.Add(position + new Vector3(1, -t, 0).normalized * this.radius);
            vertices.Add(position + new Vector3(0, -1, t).normalized * this.radius);
            vertices.Add(position + new Vector3(0, 1, t).normalized * this.radius);
            vertices.Add(position + new Vector3(0, -1, -t).normalized * this.radius);
            vertices.Add(position + new Vector3(0, 1, -t).normalized * this.radius);
            vertices.Add(position + new Vector3(t, 0, -1).normalized * this.radius);
            vertices.Add(position + new Vector3(t, 0, 1).normalized * this.radius);
            vertices.Add(position + new Vector3(-t, 0, -1).normalized * this.radius);
            vertices.Add(position + new Vector3(-t, 0, 1).normalized * this.radius);

            // And here's the formula for the 20 sides,
            // referencing the 12 vertices we just created.
            polygons.Add(new Polygon(0, 11, 5, ref planet));
            polygons.Add(new Polygon(0, 5, 1, ref planet));
            polygons.Add(new Polygon(0, 1, 7, ref planet));
            polygons.Add(new Polygon(0, 7, 10, ref planet));
            polygons.Add(new Polygon(0, 10, 11, ref planet));
            polygons.Add(new Polygon(1, 5, 9, ref planet));
            polygons.Add(new Polygon(5, 11, 4, ref planet));
            polygons.Add(new Polygon(11, 10, 2, ref planet));
            polygons.Add(new Polygon(10, 7, 6, ref planet));
            polygons.Add(new Polygon(7, 1, 8, ref planet));
            polygons.Add(new Polygon(3, 9, 4, ref planet));
            polygons.Add(new Polygon(3, 4, 2, ref planet));
            polygons.Add(new Polygon(3, 2, 6, ref planet));
            polygons.Add(new Polygon(3, 6, 8, ref planet));
            polygons.Add(new Polygon(3, 8, 9, ref planet));
            polygons.Add(new Polygon(4, 9, 5, ref planet));
            polygons.Add(new Polygon(2, 4, 11, ref planet));
            polygons.Add(new Polygon(6, 2, 10, ref planet));
            polygons.Add(new Polygon(8, 6, 7, ref planet));
            polygons.Add(new Polygon(9, 8, 1, ref planet));
        }
        /// <summary>
        /// Subdivide the Icosphere by 'subdivisions' times.
        /// </summary>
        private void Subdivide()
        {
            var mid_point_cache = new Dictionary<int, int>();

            for (int i = 0; i < subdivisions; i++)
            {
                var new_polys = new List<Polygon>();
                foreach (var poly in polygons)
                {
                    List<int> t_vertices = poly.GetVertices();
                    int a = t_vertices[0];
                    int b = t_vertices[1];
                    int c = t_vertices[2];
                    // Use GetMidPointIndex to either create a
                    // new vertex between two old vertices, or
                    // find the one that was already created.
                    int ab = GetMidPointIndex(mid_point_cache, a, b);
                    int bc = GetMidPointIndex(mid_point_cache, b, c);
                    int ca = GetMidPointIndex(mid_point_cache, c, a);
                    // Create the four new polygons using our original
                    // three vertices, and the three new midpoints.
                    new_polys.Add(new Polygon(a, ab, ca, ref planet));
                    new_polys.Add(new Polygon(b, bc, ab, ref planet));
                    new_polys.Add(new Polygon(c, ca, bc, ref planet));
                    new_polys.Add(new Polygon(ab, bc, ca, ref planet));
                }
                // Replace all our old polygons with the new set of
                // subdivided ones.
                polygons = new_polys;
            }
        }
        /// <summary>
        /// Get the Middle point.
        /// </summary>
        private int GetMidPointIndex(Dictionary<int, int> cache, int indexA, int indexB)
        {
            // We create a key out of the two original indices
            // by storing the smaller index in the upper two bytes
            // of an integer, and the larger index in the lower two
            // bytes. By sorting them according to whichever is smaller
            // we ensure that this function returns the same result
            // whether you call
            // GetMidPointIndex(cache, 5, 9)
            // or...
            // GetMidPointIndex(cache, 9, 5)
            int smaller_index = Mathf.Min(indexA, indexB);
            int greater_index = Mathf.Max(indexA, indexB);
            int key = (smaller_index << 16) + greater_index;
            // If a midpoint is already defined, just return it.
            int ret;
            if (cache.TryGetValue(key, out ret))
                return ret;
            // If we're here, it's because a midpoint for these two
            // vertices hasn't been created yet. Let's do that now!
            Vector3 p1 = vertices[indexA];
            Vector3 p2 = vertices[indexB];
            Vector3 middle = (Vector3.Lerp(p1, p2, 0.5f) - position).normalized * radius + position;

            ret = vertices.Count;
            vertices.Add(middle);

            cache.Add(key, ret);
            return ret;
        }
        /// <summary>
        /// Calculate the offset for the noise using the seed.
        /// </summary>
        private void CalculateOffset()
        {
            // Generate a random number generator to
            // always have the same random for the same seed.
            System.Random rng = new System.Random(seed);

            offset = new Vector3[generationData.Length];

            // Calculate the offset of the generation noise.
            for (int i = 0; i < offset.Length; i++)
            {
                offset[i].x = rng.Next(-10000, 10000);
                offset[i].y = rng.Next(-10000, 10000);
                offset[i].z = rng.Next(-10000, 10000);
            }
        }
        /// <summary>
        /// If the game try to quit.
        /// </summary>
        private void OnApplicationQuit()
        {
            // If the background thread is still running.
            // quit will make it stop.
            gameQuiting = true;
        }

        /// <summary>
        /// Get the style.
        /// </summary>
        /// <returns> Return the style. </returns>
        public Style GetStyle()
        {
            return terrainStyle;
        }
        /// <summary>
        /// Get the polygons list.
        /// </summary>
        /// <returns> Return the polygon list. </returns>
        public List<Polygon> GetPolygons()
        {
            return polygons;
        }
        /// <summary>
        /// Get the Color height array.
        /// </summary>
        /// <returns> Return the colorHeight array. </returns>
        public ColorHeight[] GetColorHeight()
        {
            return colorHeight;
        }
        /// <summary>
        /// Get the vertex list.
        /// </summary>
        /// <returns> Return the vertex list.</returns>
        public List<Vector3> GetVertices()
        {
            return vertices;
        }
        /// <summary>
        /// Get the sea level.
        /// </summary>
        /// <returns> Return the sea level. </returns>
        public float GetSeaLevel()
        {
            return seaLevel;
        }
        /// <summary>
        /// Get the populationData array.
        /// </summary>
        /// <returns> Return the populationData array. </returns>
        public PopulateData[] GetPopulation()
        {
            return population;
        }
        /// <summary>
        /// Get the planet center.
        /// </summary>
        /// <returns> Return the planet center. </returns>
        public Vector3 GetPosition()
        {
            return position;
        }
        /// <summary>
        /// Get material.
        /// </summary>
        /// <returns> Return the terrain material.</returns>
        public Material GetMaterial()
        {
            return material;
        }
        /// <summary>
        /// Get the sea material.
        /// </summary>
        /// <returns> Return the material for the sea.</returns>
        public Material GetSeaMaterial()
        {
            return seaMaterial;
        }
        /// <summary>
        /// Get seed.
        /// </summary>
        /// <returns> Return the seed.</returns>
        public int GetSeed()
        {
            return seed;
        }
        /// <summary>
        /// Get the chunck subdivisions.
        /// </summary>
        /// <returns> Return the chunck subdivision number.</returns>
        public int GetChunkSubdivisions()
        {
            return chunckSubdivisions;
        }
        /// <summary>
        /// Calculate the height at a specific position.
        /// </summary>
        /// <param name="position">Position where we want tto get the height.</param>
        /// <returns> Return the height.</returns>
        public float CalculateHeightAtPosition(Vector3 position)
        {
            float height = 0;

            for (int i = 0; i < generationData.Length; i++)
                height += generationData[i].CalculateNoise(position + offset[i]);

            return height;
        }
    }
}
