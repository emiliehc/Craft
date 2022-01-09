using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace dev.hongjun.mc
{
    public class TerrainGeneratorTestDriver : MonoBehaviour
    {
        private async void Start()
        {
            var terrainGenerator = new TerrainGenerator();

            var m = new Map();
            //
            // for (var y = 0; y <= 3; y++)
            // {
            //     for (var x = -y; x <= y; x++)
            //     {
            //         for (var z = -y; z <= y; z++)
            //         {
            //             //await terrainGenerator.AddVoxel(new(new(x, -y, z), Math.Abs(x * y + 100) % 6));
            //             m[x, 10 - y, z] = new Voxel(new(x, 10 - y, z), Math.Abs(x * y + 100).Mod(6));
            //         }
            //     }
            // }
            
            print("Starting terrain generation");

            const int size = 200;
            for (var z = -size; z < size; z++)
            {
                for (var x = -size; x < size; x++)
                {
                    var height = (int) (Mathf.PerlinNoise(x / 30.0f, z / 30.0f) * 20.0f + 2.0f);
                    
                    for (var y = 0; y < Mathf.Min(height, 10); y++)
                    {
                        m[x, y, z] = new Voxel(new(x, y, z), SurfaceTexture.STONE);
                    }
                    
                    for (var y = 10; y < height - 1; y++)
                    {
                        m[x, y, z] = new Voxel(new(x, y, z), SurfaceTexture.DIRT);
                    }

                    if (height > 10)
                    {
                        m[x, height - 1, z] = new Voxel(new(x, height - 1, z), SurfaceTexture.GRASS);
                    }
                }
            }

            print("Starting mesh generation");

            var tasks = new List<Task>();
            var gens = new List<TerrainGenerator>();
            foreach (var chunk in m.allChunks)
            {
                var gen = new TerrainGenerator();
                gens.Add(gen);
                tasks.Add(Task.Run(async () =>
                {
                    foreach (var voxel in chunk.voxels)
                    {
                        await gen.AddVoxel(voxel);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            foreach (var gen in gens)
            {
                gen.GenerateMesh().AddToScene(GUID.Generate().ToString());
            }
            
            print("Mesh generation ends");

            
        }
    }
}