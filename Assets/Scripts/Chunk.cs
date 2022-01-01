using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace dev.hongjun.mc
{
    public class Chunk
    {
        private const int X_SIZE = 16, Y_SIZE = 128, Z_SIZE = 16;
        private int3 chunkId;
        private readonly int3 chunkPosition;
        private Dictionary<int3, Voxel> internalVoxels;

        public Chunk(int3 id)
        {
            chunkId = id;
            chunkPosition = new(chunkId.x * X_SIZE, chunkId.y * Y_SIZE, chunkId.z * Z_SIZE);
        }

        public Voxel[] voxels => internalVoxels.Values.Select(v => new Voxel
        {
            position = v.position + chunkPosition,
            texId = v.texId
        }).ToArray();

        public bool IsInsideRelative(int3 relativePosition)
        {
            return internalVoxels.ContainsKey(relativePosition);
        }

        public bool IsInsideAbsolute(int3 absolutePosition)
        {
            return internalVoxels.ContainsKey(absolutePosition - chunkPosition);
        }
    }
}