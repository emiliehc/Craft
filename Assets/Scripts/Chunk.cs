using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace dev.hongjun.mc
{
    public class Parameters : Singleton<Parameters>
    {
        public float minLightLevel = 0.3f;
        public float maxLightLevel = 1.0f;
    }

    public static class LightLevelExt
    {
        private static readonly Parameters parameters = Parameters.Instance;
        
        public static float LightLevelAsFloat(this byte b)
        {
            return parameters.minLightLevel + b / 15.0f * (parameters.maxLightLevel - parameters.minLightLevel);
        }

        public static byte LightLevelAsByte(this float f)
        {
            return (byte)((f - parameters.minLightLevel) / (parameters.maxLightLevel - parameters.minLightLevel) * 15.0f);
        }
    }

    public class Chunk
    {
        public const int X_SIZE = 16, Y_SIZE = 128, Z_SIZE = 16;
        public int2 chunkId { get; }
        private readonly int3 chunkPosition;
        private HashSet<int3> filledVoxels = new();
        private readonly FlatArray3M<Voxel?> allVoxels = new(X_SIZE, Y_SIZE, Z_SIZE);
        private readonly FlatArray3M<byte> lightLevel = new(X_SIZE, Y_SIZE, Z_SIZE); // 0 to 15

        /// <summary>
        /// Absolute position access to voxel data
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Voxel? this[int x, int y, int z]
        {
            get => this[new(x, y, z)];
            set => this[new(x, y, z)] = value;
        }

        /// <summary>
        /// Absolute position access to voxel data
        /// </summary>
        public Voxel? this[int3 coords]
        {
            get => allVoxels[coords - chunkPosition];
            set => allVoxels[coords - chunkPosition] = value;
        }

        public byte GetLightLevelAt(int3 coords)
        {
            return lightLevel[coords - chunkPosition];
        }
        
        public void SetLightLevelAt(int3 coords, byte value)
        {
            lightLevel[coords - chunkPosition] = value;
        }

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