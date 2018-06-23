using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

namespace Generation
{
    /**
        Planet.cs
        Purpose: Generate Planets into the scene.
        Require: 'Polygon' class on the project.

        @author Mikel Jauregui
        @version 1.0.0 23/06/18 
    */
    public class Planet : MonoBehaviour
    {
        static private List<Planet> planetList = new List<Planet>();
        static private Queue<Action> queue = new Queue<Action>();
        static private bool isStartDataDone = false;
        static private bool isDataThreadingDone = false;
        static private bool isStartMeshDone = false;
        static private bool isMeshThreadingDone = false;
        static private bool isInstantiated = false;

        static private void DataThreadLoop()
        {
            // Prevent list modification during
            // thread loop.
            lock (planetList)
            {
                foreach (var item in planetList)
                {
                    // Create icosahedron.
                    item.InitAsIcosahedron();
                    // Subdivide it.
                    item.Subdivide();
                    // Create noise offset.
                    item.CalculateOffset();
                }
            }
            isDataThreadingDone = true;
        }
        static private void GeneratingMeshDataThreadLoop()
        {
            // Prevent list modification during
            // thread loop.
            lock (planetList)
            {
                foreach (var planet in planetList)
                {
                    foreach (var poly in planet.polygons)
                    {
                        // Generate mesh data.
                        poly.GenerateMesh();
                    }
                }
            }
            isMeshThreadingDone = true;
        }

        static public void AddPlanetToQueue(string name, Vector3 position, int seed, GenerationData[] generationData, Material material, float radius, int subdivisions, int chunckSubdivisions, RuntimeAnimatorController controller)
        {
            // Ensure that the program will have an apropiate behaviour.
            if (radius <= 0)
                throw new Exception("ERROR/" + name + "/Planet: Radius tried to be set to less or equal than 0.");
            if (subdivisions < 0)
                throw new Exception("ERROR/" + name + "/Planet: Subdivisions tried to be set to less than 0.");
            if (chunckSubdivisions < 0)
                throw new Exception("ERROR/" + name + "/Planet: Chunck Subdivisions tried to be set to less than 0.");
            if (material == null)
                throw new Exception("ERROR/" + name + "/Planet: NullReference to Material.");
            if (generationData == null || generationData.Length == 0)
                throw new Exception("ERROR/" + name + "/Planet: General Terrain list Empty.");

            // Instantiate a gameobject that hold planet.
            GameObject obj = new GameObject();
            obj.name = name;
            obj.transform.position = position;
            Planet planet = obj.AddComponent<Planet>();
            Animator animator = obj.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            planet.planet = planet;
            planet.InstantiatePlanetVariables(seed, generationData, material, radius, subdivisions, chunckSubdivisions);
            planet.position = position;

            // Can only be added if the thread is not working
            // with the list.
            planetList.Add(planet);
        }
        static public void StartDataQueue()
        {
            // Prevent to overload of threads. Only
            // one will be running at a time.
            if (isStartDataDone)
                return;

            // Start the thread.
            Thread thread = new Thread(DataThreadLoop);
            thread.Start();
            isStartDataDone = true;
        }
        static public void StartGeneratingMeshDataQueue()
        {
            // Only execute if the data thread has been completed.
            if (isDataThreadingDone || isStartMeshDone)
                return;

            // Start the thread.
            Thread thread = new Thread(GeneratingMeshDataThreadLoop);
            thread.Start();
            isStartMeshDone = true;
        }
        static public void InstantiateIntoWorld()
        {
            // Only execute if data and mesh data had been done.
            // And if the planet is not instantiated.
            if (!isDataThreadingDone || !isMeshThreadingDone || isInstantiated )
                return;

            foreach (var planet in planetList)
            {
                List<Polygon> poly_list = planet.GetPolygons();
                foreach (var poly in poly_list)
                {
                    // Instantiate polygone.
                    poly.Instantiate();
                }
            }
            isInstantiated = true;
        }


        private Planet planet;
        private GenerationData[] generationData;
        private Vector3[] offset;
        private Material material;
        private List<Polygon> polygons;
        private List<Vector3> vertices;
        private Vector3 position;
        private int seed;
        private float radius;
        private int subdivisions;
        private int chunckSubdivisions;

        private void InstantiatePlanetVariables(int seed, GenerationData[] generationData, Material material, float radius, int subdivisions, int chunckSubdivisions)
        {
            this.seed = seed;
            this.radius = radius;
            this.generationData = generationData;
            this.subdivisions = subdivisions;
            this.material = material;
            this.chunckSubdivisions = chunckSubdivisions;
        }
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
        private void CalculateOffset()
        {
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

        public List<Polygon> GetPolygons()
        {
            return polygons;
        }
        public List<Vector3> GetVertices()
        {
            return vertices;
        }
        public Vector3 GetPosition()
        {
            return position;
        }
        public Material GetMaterial()
        {
            return material;
        }
        public int GetChunkSubdivisions()
        {
            return chunckSubdivisions;
        }
        public float CalculateHeightAtPosition(Vector3 position)
        {
            float height = 0;

            for (int i = 0; i < generationData.Length; i++)
                height += generationData[i].CalculateNoise(position + offset[i]);

            return height;
        }
    }
}
