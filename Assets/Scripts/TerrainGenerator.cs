using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace dev.hongjun.mc
{
    public class TerrainGenerator
    {
        private static readonly Vector3[] cube = {
            // back (negative x)
            new(-0.5f, 0.5f, 0.5f),
            new(-0.5f, 0.5f, -0.5f),
            new(-0.5f, -0.5f, -0.5f),
            new(-0.5f, -0.5f, 0.5f),
            
            // front (positive x)
            new(0.5f, -0.5f, -0.5f),
            new(0.5f, 0.5f, -0.5f),
            new(0.5f, 0.5f, 0.5f),
            new(0.5f, -0.5f, 0.5f),
            
            // left (negative z)
            new(0.5f, 0.5f, -0.5f),
            new(0.5f, -0.5f, -0.5f),
            new(-0.5f, -0.5f, -0.5f),
            new(-0.5f, 0.5f, -0.5f),
            
            // right (positive z)
            new(-0.5f, -0.5f, 0.5f),
            new(0.5f, -0.5f, 0.5f),
            new(0.5f, 0.5f, 0.5f),
            new(-0.5f, 0.5f, 0.5f),
            
            // bottom (negative y)
            new(-0.5f, -0.5f, -0.5f),
            new(0.5f, -0.5f, -0.5f),
            new(0.5f, -0.5f, 0.5f),
            new(-0.5f, -0.5f, 0.5f),
            
            // top (positive y)
            new(0.5f, 0.5f, 0.5f),
            new(0.5f, 0.5f, -0.5f),
            new(-0.5f, 0.5f, -0.5f),
            new(-0.5f, 0.5f, 0.5f),
            
            // new Vector3(-0.5f, -0.5f, -0.5f),
            // new Vector3(0.5f, -0.5f, -0.5f),
            // new Vector3(0.5f, 0.5f, -0.5f),
            // new Vector3(-0.5f, 0.5f, -0.5f),
            // new Vector3(-0.5f, 0.5f, 0.5f),
            // new Vector3(0.5f, 0.5f, 0.5f),
            // new Vector3(0.5f, -0.5f, 0.5f),
            // new Vector3(-0.5f, -0.5f, 0.5f),
        };

        private static readonly int[] faceTriangles =
        {
            0, 1, 2,
            2, 3, 0,
        };

        private readonly List<Voxel> voxels = new();

        public void AddVoxel(in Voxel voxel)
        {
            voxels.Add(voxel);
        }

        public Mesh GenerateMesh()
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();
            List<Vector2> uv = new();

            var indexOffset = 0;

            foreach (var voxel in voxels)
            {
                vertices.AddRange(cube.Select(vertex => voxel.position + vertex));
                
                for (var i = 0; i < 6; i++) // per face
                {
                    uv.AddRange(voxel.texId.GetUv());
                    triangles.AddRange(faceTriangles.Select(index => index + indexOffset));
                    indexOffset += 4;
                }
            }

            var mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uv.ToArray(),
            };

            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            
            return mesh;
        }
    }
}
