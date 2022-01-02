using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace dev.hongjun.mc
{
    public class Chunk
    {
        public const int X_SIZE = 16, Y_SIZE = 128, Z_SIZE = 16;
        public int2 chunkId { get; }
        private readonly int3 chunkPosition;
        private HashSet<int3> filledVoxels = new();
        private readonly FlatArray3M<Voxel?> allVoxels = new(X_SIZE, Y_SIZE, Z_SIZE);

        /// <summary>
        /// Absolute position access
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public ref Voxel? this[int x, int y, int z] => ref allVoxels[new int3(x, y, z) - chunkPosition];

        public Chunk(int2 id)
        {
            chunkId = id;
            chunkPosition = new(chunkId.x * X_SIZE, 0, chunkId.y * Z_SIZE);
        }

        public IEnumerable<Voxel> voxels => allVoxels.Where(v => v.HasValue).Select(v => v.Value).ToArray();

        public bool IsInsideRelative(int3 relativePosition)
        {
            return filledVoxels.Contains(relativePosition);
        }

        public bool IsInsideAbsolute(int3 absolutePosition)
        {
            return filledVoxels.Contains(absolutePosition - chunkPosition);
        }
    }
}