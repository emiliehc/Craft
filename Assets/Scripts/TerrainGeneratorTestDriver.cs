using System;
using UnityEngine;

namespace dev.hongjun.mc
{
    public class TerrainGeneratorTestDriver : MonoBehaviour
    {
        private void Start()
        {
            var terrainGenerator = new TerrainGenerator();

            for (var y = 0; y <= 3; y++)
            {
                for (var x = -y; x <= y; x++)
                {
                    for (var z = -y; z <= y; z++)
                    {
                        terrainGenerator.AddVoxel(new(new(x, -y, z), Math.Abs(x + z + 20) % 6));
                    }
                }
            }

            var obj = new GameObject
            {
                name = "Beacon"
            };

            var mesh = terrainGenerator.GenerateMesh();
            mesh.name = Guid.NewGuid().ToString();

            obj.AddComponent<MeshFilter>();
            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.AddComponent<MeshRenderer>();
            obj.AddComponent<Rigidbody>();
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.AddComponent<MeshCollider>();
            obj.GetComponent<MeshCollider>().sharedMesh = mesh;
            var material = new Material(Resources.Load<Shader>("Shaders/ChunkShader"));
            material.SetTexture("_MasterTexture", TextureManager.Instance.masterTexture);
            obj.GetComponent<Renderer>().material = material;
        }
    }
}