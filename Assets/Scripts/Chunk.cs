using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace dev.hongjun.mc
{
    public class Chunk
    {
        private const int X_SIZE = 16, Y_SIZE = 128, Z_SIZE = 16;
        private Vector3Int chunkId;
        private readonly Vector3Int chunkPosition;
        private Dictionary<Vector3Int, Voxel> internalVoxels;

        public Chunk(Vector3Int id)
        {
            chunkId = id;
            chunkPosition = new Vector3Int(chunkId.x * X_SIZE, chunkId.y * Y_SIZE, chunkId.z * Z_SIZE);
        }

        public Voxel[] voxels => internalVoxels.Values.Select(v => new Voxel
        {
            position = v.position + chunkPosition,
            texId = v.texId
        }).ToArray();

        public bool IsInsideRelative(Vector3Int relativePosition)
        {
            return internalVoxels.ContainsKey(relativePosition);
        }

        public bool IsInsideAbsolute(Vector3Int absolutePosition)
        {
            return internalVoxels.ContainsKey(absolutePosition - chunkPosition);
        }
    }
}