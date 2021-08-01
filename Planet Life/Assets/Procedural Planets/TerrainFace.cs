using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{

    ShapeGenerator shapeGenerator;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public Mesh oceanMesh;

    int ocean_resolution;
    float Ocean;
    float Offset;
    float OceanUp;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp, float Ocean, int ocean_resolution, float Offset,float OceanUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.Ocean = Ocean;
        this.OceanUp = OceanUp;
        this.ocean_resolution = ocean_resolution;
        this.Offset = Offset;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                float Calculation = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);
                if (Calculation / shapeGenerator.settings.planetRadius < Ocean)
                {
                    Calculation -= 10;
                }

                vertices[i] = Calculation * pointOnUnitSphere;

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;

                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        Vector3[] o_vertices = new Vector3[ocean_resolution * ocean_resolution];
        int[] o_triangles = new int[(ocean_resolution - 1) * (ocean_resolution - 1) * 6];
        int o_triIndex = 0;

        for (int y = 0; y < ocean_resolution; y++)
        {
            for (int x = 0; x < ocean_resolution; x++)
            {
                int i = x + y * ocean_resolution;
                Vector2 percent = new Vector2(x, y) / (ocean_resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                float Calculation = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);
                o_vertices[i] = pointOnUnitSphere * shapeGenerator.settings.planetRadius * OceanUp;

                if (Calculation / shapeGenerator.settings.planetRadius < Ocean + Offset)
                {
                    if (x != ocean_resolution - 1 && y != ocean_resolution - 1)
                    {
                        o_triangles[o_triIndex] = i;
                        o_triangles[o_triIndex + 1] = i + ocean_resolution + 1;
                        o_triangles[o_triIndex + 2] = i + ocean_resolution;

                        o_triangles[o_triIndex + 3] = i;
                        o_triangles[o_triIndex + 4] = i + 1;
                        o_triangles[o_triIndex + 5] = i + ocean_resolution + 1;
                        o_triIndex += 6;

                    }
                }
            }
        }
        oceanMesh = new Mesh();
        oceanMesh.Clear();
        oceanMesh.vertices = o_vertices;
        oceanMesh.triangles = o_triangles;
        oceanMesh.RecalculateNormals();
    }
}
