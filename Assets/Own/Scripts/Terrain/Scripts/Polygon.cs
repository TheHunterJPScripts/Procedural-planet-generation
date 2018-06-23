using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class Polygon
    {
        static private void ListMiddlePoints(List<Vector3> list, Vector3 a, Vector3 b, int sub)
        {
            Vector3 new_vector = new Vector3();

            new_vector.x = (b.x - a.x) / sub;
            new_vector.y = (b.y - a.y) / sub;
            new_vector.z = (b.z - a.z) / sub;

            for (int i = 1; i < sub; i++)
                list.Add(a + new_vector * i);
        }
        static public MeshData TriangleToStair(Vector3[] triangle, Vector3 middle)
        {
            MeshData mesh_data = new MeshData();
            Vector3 v1, v2, v3;

            v1 = triangle[0];
            v2 = triangle[1];
            v3 = triangle[2];
            Organize(ref v1, ref v2, ref v3);

            float h1, h2, h3;
            h1 = Vector3.Magnitude(v1);
            h2 = Vector3.Magnitude(v2);
            h3 = Vector3.Magnitude(v3);

            int h_min, h_max;
            h_min = Mathf.FloorToInt(Mathf.Min(h1, h2, h3));
            h_max = Mathf.FloorToInt(Mathf.Max(h1, h2, h3));
            int iv = 0;
            for (int h = h_min; h <= h_max; h++)
            {
                int points_above = 0;
                if (h3 == h2 && h3 == h1)
                    points_above = 3;
                else if (h3 > h)
                    points_above = 3;
                else if (h2 > h)
                    points_above = 2;
                else
                    points_above = 1;

                // Current Plane.
                Vector3 v1_c, v2_c, v3_c;
                v1_c = v1.normalized * h;
                v2_c = v2.normalized * h;
                v3_c = v3.normalized * h;

                // Previous Plane.
                Vector3 v1_p, v2_p, v3_p;
                v1_p = v1.normalized * (h - 1);
                v2_p = v2.normalized * (h - 1);
                v3_p = v3.normalized * (h - 1);

                Vector3 normal = Vector3.Cross(v1_p - v2_p, v3_p - v2_p).normalized;
                float d = -Vector3.Dot(normal, v1_p);
                float sign1 = d;
                float distance = Mathf.Abs(normal.x * v1_c.x + normal.y * v1_c.y + normal.z * v1_c.z + d);

                //float a = h_min % 2;
                Color color;
                if (h > 116)
                    color = new Color(0.6415094f, 0.5063629f, 0.1845852f, 1);
                else if (h > 115)
                    color = new Color(0.4470589f, 0.6392157f, 0.145098f, 1);
                else if (h > 114)
                    color = new Color(0.5254902f, 0.6431373f, 0.1333333f, 1);
                else if (h > 113)
                    color = new Color(0.6196079f, 0.6431373f, 0.1568628f, 1);
                else if (h > 112)
                    color = new Color(0.7176471f, 0.6588235f, 0.1686275f, 1);
                else
                    color = new Color(0.9450981f, 0.8392158f, 0.4980392f, 1);


                if (points_above == 3)
                {
                    // Floor.
                    mesh_data.vertices.Add(v1_c + middle);
                    mesh_data.vertices.Add(v2_c + middle);
                    mesh_data.vertices.Add(v3_c + middle);

                    mesh_data.colors.Add(color);
                    mesh_data.colors.Add(color);
                    mesh_data.colors.Add(color);

                    if (sign1 < 0)
                    {
                        // Floor.
                        mesh_data.triangles.Add(iv);
                        mesh_data.triangles.Add(iv + 2);
                        mesh_data.triangles.Add(iv + 1);
                    }
                    else
                    {
                        mesh_data.triangles.Add(iv);
                        mesh_data.triangles.Add(iv + 1);
                        mesh_data.triangles.Add(iv + 2);
                    }
                    iv += 3;
                }
                else if (points_above == 2)
                {
                    float t1 = (h1 - h) / (h1 - h3);
                    Vector3 v1_v3_c = Vector3.Lerp(v1, v3, t1);
                    Vector3 v1_v3_p = v1_v3_c + Vector3.Cross(v1_c - v2_c, v3_c - v2_c).normalized * distance * Mathf.Sign(sign1);

                    float t2 = (h2 - h) / (h2 - h3);
                    Vector3 v2_v3_c = Vector3.Lerp(v2, v3, t2);
                    Vector3 v2_v3_p = v2_v3_c + Vector3.Cross(v1_c - v2_c, v3_c - v2_c).normalized * distance * Mathf.Sign(sign1);

                    if (sign1 < 0)
                    {
                        // Floor.
                        mesh_data.vertices.Add(v1_v3_c + middle);
                        mesh_data.vertices.Add(v2_v3_c + middle);
                        mesh_data.vertices.Add(v1_c + middle);
                        mesh_data.vertices.Add(v2_c + middle);

                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);

                        mesh_data.triangles.Add(iv);
                        mesh_data.triangles.Add(iv + 1);
                        mesh_data.triangles.Add(iv + 2);

                        mesh_data.triangles.Add(iv + 1);
                        mesh_data.triangles.Add(iv + 3);
                        mesh_data.triangles.Add(iv + 2);
                        iv += 4;

                        // Wall.
                        mesh_data.vertices.Add(v1_v3_c + middle);
                        mesh_data.vertices.Add(v1_v3_p + middle);
                        mesh_data.vertices.Add(v2_v3_c + middle);
                        mesh_data.vertices.Add(v2_v3_p + middle);

                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);

                        mesh_data.triangles.Add(iv);
                        mesh_data.triangles.Add(iv + 1);
                        mesh_data.triangles.Add(iv + 2);

                        mesh_data.triangles.Add(iv + 1);
                        mesh_data.triangles.Add(iv + 3);
                        mesh_data.triangles.Add(iv + 2);
                        iv += 4;
                    }
                    else
                    {
                        // Floor.
                        mesh_data.vertices.Add(v1_v3_c + middle);
                        mesh_data.vertices.Add(v2_v3_c + middle);
                        mesh_data.vertices.Add(v1_c + middle);
                        mesh_data.vertices.Add(v2_c + middle);

                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);

                        mesh_data.triangles.Add(iv);
                        mesh_data.triangles.Add(iv + 2);
                        mesh_data.triangles.Add(iv + 1);

                        mesh_data.triangles.Add(iv + 1);
                        mesh_data.triangles.Add(iv + 2);
                        mesh_data.triangles.Add(iv + 3);
                        iv += 4;

                        // Wall.
                        mesh_data.vertices.Add(v1_v3_c + middle);
                        mesh_data.vertices.Add(v1_v3_p + middle);
                        mesh_data.vertices.Add(v2_v3_c + middle);
                        mesh_data.vertices.Add(v2_v3_p + middle);

                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);

                        mesh_data.triangles.Add(iv);
                        mesh_data.triangles.Add(iv + 2);
                        mesh_data.triangles.Add(iv + 1);

                        mesh_data.triangles.Add(iv + 1);
                        mesh_data.triangles.Add(iv + 2);
                        mesh_data.triangles.Add(iv + 3);
                        iv += 4;
                    }
                }
                else if (points_above == 1)
                {
                    float t1 = (h1 - h) / (h1 - h3);
                    Vector3 v1_v3_c = Vector3.Lerp(v1, v3, t1);
                    Vector3 v1_v3_p = v1_v3_c + Vector3.Cross(v1_c - v2_c, v3_c - v2_c).normalized * distance * Mathf.Sign(sign1);

                    float t2 = (h1 - h) / (h1 - h2);
                    Vector3 v2_v3_c = Vector3.Lerp(v1, v2, t2);
                    Vector3 v2_v3_p = v2_v3_c + Vector3.Cross(v1_c - v2_c, v3_c - v2_c).normalized * distance * Mathf.Sign(sign1);

                    if (sign1 < 0)
                    {
                        // Floor.
                        mesh_data.vertices.Add(v1_v3_c + middle);
                        mesh_data.vertices.Add(v2_v3_c + middle);
                        mesh_data.vertices.Add(v1_c + middle);

                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);

                        mesh_data.triangles.Add(iv);
                        mesh_data.triangles.Add(iv + 1);
                        mesh_data.triangles.Add(iv + 2);

                        iv += 3;

                        // Wall.
                        mesh_data.vertices.Add(v1_v3_c + middle);
                        mesh_data.vertices.Add(v1_v3_p + middle);
                        mesh_data.vertices.Add(v2_v3_c + middle);
                        mesh_data.vertices.Add(v2_v3_p + middle);

                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);

                        mesh_data.triangles.Add(iv);
                        mesh_data.triangles.Add(iv + 1);
                        mesh_data.triangles.Add(iv + 2);

                        mesh_data.triangles.Add(iv + 1);
                        mesh_data.triangles.Add(iv + 3);
                        mesh_data.triangles.Add(iv + 2);
                        iv += 4;
                    }
                    else
                    {
                        // Floor.
                        mesh_data.vertices.Add(v1_v3_c + middle);
                        mesh_data.vertices.Add(v2_v3_c + middle);
                        mesh_data.vertices.Add(v1_c + middle);

                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);

                        mesh_data.triangles.Add(iv);
                        mesh_data.triangles.Add(iv + 2);
                        mesh_data.triangles.Add(iv + 1);

                        iv += 3;

                        // Wall.
                        mesh_data.vertices.Add(v1_v3_c + middle);
                        mesh_data.vertices.Add(v1_v3_p + middle);
                        mesh_data.vertices.Add(v2_v3_c + middle);
                        mesh_data.vertices.Add(v2_v3_p + middle);

                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);
                        mesh_data.colors.Add(color);

                        mesh_data.triangles.Add(iv);
                        mesh_data.triangles.Add(iv + 2);
                        mesh_data.triangles.Add(iv + 1);

                        mesh_data.triangles.Add(iv + 1);
                        mesh_data.triangles.Add(iv + 2);
                        mesh_data.triangles.Add(iv + 3);
                        iv += 4;
                    }
                }
            }
            MeshData data = new MeshData();
            data.vertices = new List<Vector3>(triangle);
            int[] i = new int[3] { 0, 1, 2 };
            data.triangles = new List<int>(i);

            return mesh_data;
        }
        static void Organize(ref Vector3 a, ref Vector3 b, ref Vector3 c)
        {
            Vector3 v1, v2, v3;
            float h1, h2, h3;

            h1 = Vector3.Magnitude(a);
            h2 = Vector3.Magnitude(b);
            h3 = Vector3.Magnitude(c);

            if (h1 > h2)
            {
                if (h1 > h3)
                {
                    v1 = a;
                    if (h2 > h3)
                    {
                        v2 = b;
                        v3 = c;
                    }
                    else
                    {
                        v2 = c;
                        v3 = b;
                    }
                }
                else
                {
                    v1 = c;
                    v2 = a;
                    v3 = b;
                }
            }
            else
            {
                if (h2 > h3)
                {
                    v1 = b;
                    if (h1 > h3)
                    {
                        v2 = a;
                        v3 = c;
                    }
                    else
                    {
                        v2 = c;
                        v3 = a;
                    }
                }
                else
                {
                    v1 = c;
                    v2 = b;
                    v3 = a;
                }
            }
            a = v1;
            b = v2;
            c = v3;
        }

        private Planet planet;
        private GameObject face;
        private MeshData meshData;
        private List<int> vertices;

        private float getHeight(Vector3 p)
        {
            return planet.CalculateHeightAtPosition(p);
        }
        public void Instantiate()
        {
            // Instantiate the gameobject.
            face = new GameObject();
            face.transform.parent = planet.transform;
            MeshFilter mesh_filter = face.AddComponent<MeshFilter>();
            MeshCollider collider = face.AddComponent<MeshCollider>();
            MeshRenderer renderer = face.AddComponent<MeshRenderer>();
            renderer.material = planet.GetMaterial();

            // Apply the mesh to the object.
            Mesh mesh = new Mesh();
            mesh.vertices = meshData.vertices.ToArray();
            mesh.triangles = meshData.triangles.ToArray();
            mesh.colors = meshData.colors.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh_filter.mesh = mesh;
            collider.sharedMesh = mesh;
        }
        private List<List<Vector3>> Subdivide(List<int> m_Vertices)
        {
            List<List<Vector3>> vertices = new List<List<Vector3>>();
            List<Vector3> p_Vertices = planet.GetVertices();
            List<Vector3> left = new List<Vector3>();
            List<Vector3> right = new List<Vector3>();
            vertices = new List<List<Vector3>>();

            Vector3 a = p_Vertices[m_Vertices[0]] - planet.GetPosition();
            Vector3 b = p_Vertices[m_Vertices[1]] - planet.GetPosition();
            Vector3 c = p_Vertices[m_Vertices[2]] - planet.GetPosition();

            // Amount of sections we need to create.
            int size = (int)Mathf.Pow(2, planet.GetChunkSubdivisions());

            // Create the sections of the left side of the triangle.
            left.Add(a);
            ListMiddlePoints(left, a, c, size);
            left.Add(c);

            // Create the sections of the right side of the triangle.
            right.Add(b);
            ListMiddlePoints(right, b, c, size);
            right.Add(c);

            // Calculate each slice points.
            for (int i = 0; i < size; i++)
            {
                vertices.Add(new List<Vector3>());
                vertices[i].Add(left[i]);
                ListMiddlePoints(vertices[i], left[i], right[i], size - i);
                vertices[i].Add(right[i]);
            }

            // Add the last slice.
            vertices.Add(new List<Vector3>());
            vertices[vertices.Count - 1].Add(c);

            return vertices;
        }

        public List<int> GetVertices()
        {
            return vertices;
        }
        public Polygon(int a, int b, int c, ref Planet planet)
        {
            vertices = new List<int>() { a, b, c };
            this.planet = planet;
        }
        public void GenerateMesh()
        {
            List<Vector3> t_vertices = new List<Vector3>();
            List<int> t_triangles = new List<int>();
            List<Color> t_colors = new List<Color>();

            // If the chunck don't need to be divided.
            if(planet.GetChunkSubdivisions() == 0)
            {
                // Only asssign the original triangle.
                List<Vector3> v = planet.GetVertices();
                t_vertices = new List<Vector3>() { v[vertices[0]], v[vertices[1]], v[vertices[2]] };
                t_triangles = new List<int>() { 0,1,2 };
                meshData = new MeshData(t_vertices, t_triangles, t_colors);

                return;
            }

            List<List<Vector3>> sub_vertices = Subdivide(vertices);
            List<Mesh> meshes = new List<Mesh>();
            int size = (int)Mathf.Pow(2, planet.GetChunkSubdivisions());
            int iv = 0;

            // Create the mesh data.
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size - x; y++)
                {
                    Vector3 v1, v2, v3, v4;

                    v1 = sub_vertices[x][y];
                    v2 = sub_vertices[x][y + 1];
                    v3 = sub_vertices[x + 1][y];

                    // Variables for the height of each vertex of the triangle.
                    float h1 = 0.0f, h2 = 0.0f, h3 = 0.0f, h4 = 0.0f;

                    h1 += getHeight(v1);
                    h2 += getHeight(v2);
                    h3 += getHeight(v3);

                    v1 = v1 + v1.normalized * h1;
                    v2 = v2 + v2.normalized * h2;
                    v3 = v3 + v3.normalized * h3;

                    bool low_poly = true;

                    if(low_poly)
                    {
                        t_vertices.Add(v1);
                        t_vertices.Add(v2);
                        t_vertices.Add(v3);

                        t_triangles.Add(iv + 0);
                        t_triangles.Add(iv + 1);
                        t_triangles.Add(iv + 2);

                        if (h1 > 19 && h2 > 19 && h3 > 19)
                        {
                            t_colors.Add(new Color(0.6415094f, 0.5063629f, 0.1845852f, 1));
                            t_colors.Add(new Color(0.6415094f, 0.5063629f, 0.1845852f, 1));
                            t_colors.Add(new Color(0.6415094f, 0.5063629f, 0.1845852f, 1));
                        }
                        else if (h1 > 16 && h2 > 16 && h3 > 16)
                        {
                            t_colors.Add(new Color(0.4470589f, 0.6392157f, 0.145098f, 1));
                            t_colors.Add(new Color(0.4470589f, 0.6392157f, 0.145098f, 1));
                            t_colors.Add(new Color(0.4470589f, 0.6392157f, 0.145098f, 1));
                        }
                        else if (h1 > 14 && h2 > 14 && h3 > 14)
                        {
                            t_colors.Add(new Color(0.6196079f, 0.6431373f, 0.1568628f, 1));
                            t_colors.Add(new Color(0.6196079f, 0.6431373f, 0.1568628f, 1));
                            t_colors.Add(new Color(0.6196079f, 0.6431373f, 0.1568628f, 1));
                        }
                        else
                        {
                            t_colors.Add(new Color(0.9450981f, 0.8392158f, 0.4980392f, 1));
                            t_colors.Add(new Color(0.9450981f, 0.8392158f, 0.4980392f, 1));
                            t_colors.Add(new Color(0.9450981f, 0.8392158f, 0.4980392f, 1));
                        }

                        iv += 3;
                    }
                    else
                    {
                        Vector3[] triangle = new Vector3[3] { v1, v2, v3 };
                        MeshData mesh_data = TriangleToStair(triangle, planet.GetPosition());

                        foreach (var item in mesh_data.vertices)
                        {
                            t_vertices.Add(item);
                        }
                        foreach (var item in mesh_data.triangles)
                        {
                            t_triangles.Add(iv + item);
                        }
                        foreach (var item in mesh_data.colors)
                        {
                            t_colors.Add(item);
                        }

                        iv += mesh_data.vertices.Count;
                    }
                    
                    if (x != 0)
                    {
                        v4 = sub_vertices[x - 1][y + 1];

                        h4 += getHeight(v4);

                        v4 = v4 + v4.normalized * h4;

                        if(low_poly)
                        {
                            t_vertices.Add(v1);
                            t_vertices.Add(v2);
                            t_vertices.Add(v4);

                            t_triangles.Add(iv + 0);
                            t_triangles.Add(iv + 2);
                            t_triangles.Add(iv + 1);

                            if (h1 > 19 && h2 > 19 && h4 > 19)
                            {
                                t_colors.Add(new Color(0.6415094f, 0.5063629f, 0.1845852f, 1));
                                t_colors.Add(new Color(0.6415094f, 0.5063629f, 0.1845852f, 1));
                                t_colors.Add(new Color(0.6415094f, 0.5063629f, 0.1845852f, 1));
                            }
                            else if (h1 > 16 && h2 > 16 && h4 > 16)
                            {
                                t_colors.Add(new Color(0.4470589f, 0.6392157f, 0.145098f, 1));
                                t_colors.Add(new Color(0.4470589f, 0.6392157f, 0.145098f, 1));
                                t_colors.Add(new Color(0.4470589f, 0.6392157f, 0.145098f, 1));
                            }
                            else if (h1 > 14 && h2 > 14 && h4 > 14)
                            {
                                t_colors.Add(new Color(0.6196079f, 0.6431373f, 0.1568628f, 1));
                                t_colors.Add(new Color(0.6196079f, 0.6431373f, 0.1568628f, 1));
                                t_colors.Add(new Color(0.6196079f, 0.6431373f, 0.1568628f, 1));
                            }
                            else
                            {
                                t_colors.Add(new Color(0.9450981f, 0.8392158f, 0.4980392f, 1));
                                t_colors.Add(new Color(0.9450981f, 0.8392158f, 0.4980392f, 1));
                                t_colors.Add(new Color(0.9450981f, 0.8392158f, 0.4980392f, 1));
                            }

                            iv += 3;
                        }
                        else
                        {
                            Vector3[] triangle2 = new Vector3[3] { v1, v2, v4 };

                            MeshData mesh_data2 = TriangleToStair(triangle2, planet.GetPosition());

                            foreach (var item in mesh_data2.vertices)
                            {
                                t_vertices.Add(item);
                            }
                            foreach (var item in mesh_data2.triangles)
                            {
                                t_triangles.Add(iv + item);
                            }
                            foreach (var item in mesh_data2.colors)
                            {
                                t_colors.Add(item);
                            }
                            iv += mesh_data2.vertices.Count;
                        }
                        
                    }
                }
            }
            // Assign the mesh data.
            meshData = new MeshData(t_vertices, t_triangles, t_colors);
        }

    }
    public class MeshData
    {
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Color> colors;

        public MeshData()
        {
            this.vertices = new List<Vector3>();
            this.triangles = new List<int>();
            this.colors = new List<Color>();
        }
        public MeshData(List<Vector3> vertices, List<int> triangles, List<Color> colors)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.colors = colors;
        }
    }
}