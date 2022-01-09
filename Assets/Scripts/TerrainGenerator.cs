using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using static Unity.Mathematics.math;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace dev.hongjun.mc
{
    public class TerrainGenerator
    {
        private readonly Dictionary<(int3, CubeFace), Surface> surfaces = new();
        private readonly HashSet<int3> setOfExistingVoxels = new();

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

        public async Task AddVoxel(Voxel voxel)
        {
            await Task.Yield();
            voxels.Add(voxel);
            
            List<Surface> newSurfaces = new();
            
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
                
                newSurfaces.AddRange(surfaceIndices
                    .Select(i => new Surface
                    {
                        position = voxel.position, 
                        face = (CubeFace) i, 
                        texture = voxel.texId,
                        light = 1.0f,
                    }));

                newSurfaces.ForEach(surface => surfaces.Add((surface.position, surface.face), surface));

                setOfExistingVoxels.Add(pos);
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 7 * sizeof(float))]
        private struct Vertex
        {
            [FieldOffset(0)]
            public float4 position;

            [FieldOffset(sizeof(float) * 4)] 
            public float2 uv;

            [FieldOffset(sizeof(float) * 6)] 
            public float light;
        }


        public Mesh GenerateMesh()
        {
            var vertexCount = surfaces.Count * 4;
            
            List<int> triangles = new(surfaces.Count * 6);

            var vertexBuffer = new List<Vertex>(surfaces.Count * 4);
            {
                var indexOffset = 0;

                foreach (var surface in surfaces.Select(posSurface => posSurface.Value))
                {
                    var basePositions = surface.face.GetUnitVertices();
                    var posOffset = surface.position.ToPoint();
                    var surfaceUv = surface.texture.GetUv();
                    for (var i = 0; i < 4; i++)
                    {
                        vertexBuffer.Add(new()
                        {
                            position = posOffset + basePositions[i],
                            uv = surfaceUv[i],
                            light = surface.light,
                        });
                    }

                    triangles.AddRange(faceTriangles.Select(index => index + indexOffset));
                    indexOffset += 4;
                }
            }

            var mesh = new Mesh();
            var layout = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 4),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float32, 1)
            };
            
            mesh.SetVertexBufferParams(vertexCount, layout);

            mesh.SetVertexBufferData(vertexBuffer, 0, 0, vertexCount);
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            
            mesh.Optimize();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            
            mesh.name = Guid.NewGuid().ToString();

            return mesh;
        }
    }

    public static class MeshExt
    {
        public static void AddToScene(this Mesh mesh, string name)
        {
            var obj = new GameObject
            {
                name = name,
            };
            
            obj.AddComponent<MeshFilter>();
            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.AddComponent<MeshRenderer>();
            obj.AddComponent<Rigidbody>();
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.AddComponent<MeshCollider>();
            obj.GetComponent<MeshCollider>().sharedMesh = mesh;
            var material = new Material(Resources.Load<Shader>("Shaders/LitChunkShader"));
            material.SetTexture("_MainTex", TextureManager.Instance.masterTexture);
            obj.GetComponent<Renderer>().material = material;
        }
    }
}