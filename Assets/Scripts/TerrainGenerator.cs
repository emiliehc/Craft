using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Mathematics.math;
using Unity.Mathematics;

namespace dev.hongjun.mc
{
    public class TerrainGenerator
    {
        // private static readonly Vector3[] cube =
        // {
        //     // back (negative x)
        //     new(-0.5f, -0.5f, 0.5f),
        //     new(-0.5f, 0.5f, 0.5f),
        //     new(-0.5f, 0.5f, -0.5f),
        //     new(-0.5f, -0.5f, -0.5f),
        //
        //     // front (positive x)
        //     new(0.5f, -0.5f, -0.5f),
        //     new(0.5f, 0.5f, -0.5f),
        //     new(0.5f, 0.5f, 0.5f),
        //     new(0.5f, -0.5f, 0.5f),
        //
        //     // bottom (negative y)
        //     new(-0.5f, -0.5f, 0.5f),
        //     new(-0.5f, -0.5f, -0.5f),
        //     new(0.5f, -0.5f, -0.5f),
        //     new(0.5f, -0.5f, 0.5f),
        //
        //     // top (positive y)
        //     new(0.5f, 0.5f, 0.5f),
        //     new(0.5f, 0.5f, -0.5f),
        //     new(-0.5f, 0.5f, -0.5f),
        //     new(-0.5f, 0.5f, 0.5f),
        //
        //     // left (negative z)
        //     new(-0.5f, -0.5f, -0.5f),
        //     new(-0.5f, 0.5f, -0.5f),
        //     new(0.5f, 0.5f, -0.5f),
        //     new(0.5f, -0.5f, -0.5f),
        //
        //     // right (positive z)
        //     new(0.5f, -0.5f, 0.5f),
        //     new(0.5f, 0.5f, 0.5f),
        //     new(-0.5f, 0.5f, 0.5f),
        //     new(-0.5f, -0.5f, 0.5f),
        //
        //     // new Vector3(-0.5f, -0.5f, -0.5f),
        //     // new Vector3(0.5f, -0.5f, -0.5f),
        //     // new Vector3(0.5f, 0.5f, -0.5f),
        //     // new Vector3(-0.5f, 0.5f, -0.5f),
        //     // new Vector3(-0.5f, 0.5f, 0.5f),
        //     // new Vector3(0.5f, 0.5f, 0.5f),
        //     // new Vector3(0.5f, -0.5f, 0.5f),
        //     // new Vector3(-0.5f, -0.5f, 0.5f),
        // };

        private static readonly int[] faceTriangles =
        {
            0, 1, 2,
            2, 3, 0,
        };

        private static readonly int3[] directions =
        {
            (int3) left(),
            (int3) right(),
            (int3) down(),
            (int3) up(),
            (int3) back(),
            (int3) forward(),
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

            Dictionary<(int3, CubeFace), Surface> surfaces = new();

            HashSet<int3> setOfExistingVoxels = new();

            {
                List<Surface> newSurfaces = new();
                foreach (var voxel in voxels)
                {
                    var pos = voxel.position;

                    HashSet<int> surfaceIndices = new()
                    {
                        0, 1, 2, 3, 4, 5,
                    };

                    for (var i = 0; i < 6; i++)
                    {
                        var direction = directions[i];
                        var adjacentPos = pos + direction;
                        if (setOfExistingVoxels.Contains(adjacentPos))
                        {
                            surfaceIndices.Remove(i);
                        }
                        
                        // remove existing surface
                        surfaces.Remove((adjacentPos, ((CubeFace) i).Opposite()));
                    }
                    
                    newSurfaces.Clear();
                    newSurfaces.AddRange(surfaceIndices
                        .Select(i => new Surface
                    {
                        position = voxel.position, 
                        face = (CubeFace) i, 
                        texture = voxel.texId,
                    }));

                    newSurfaces.ForEach(surface => surfaces.Add((surface.position, surface.face), surface));

                    setOfExistingVoxels.Add(pos);
                }
            }


            {
                var indexOffset = 0;

                // foreach (var voxel in voxels)
                // {
                //     vertices.AddRange(cube.Select(vertex => (Vector3)(float3)voxel.position + vertex));
                //     
                //     for (var i = 0; i < 6; i++) // per face
                //     {
                //         uv.AddRange(voxel.texId.GetUv());
                //         triangles.AddRange(faceTriangles.Select(index => index + indexOffset));
                //         indexOffset += 4;
                //     }
                // }

                foreach (var surface in surfaces.Select(posSurface => posSurface.Value))
                {
                    vertices.AddRange(surface.face.GetUnitVertices()
                        .Select(vertex => (Vector3) (float3) surface.position + vertex));
                    uv.AddRange(surface.texture.GetUv());
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