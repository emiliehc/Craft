using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace dev.hongjun.mc
{
    public class Map
    {
        private readonly Dictionary<int2, Chunk> chunks = new();

        public Voxel? this[int x, int y, int z] {
            get => GetChunkByPosition(new (x, y, z))[x, y, z]; 
            set => GetChunkByPosition(new (x, y, z))[x, y, z] = value;
        }

        public Voxel? this[int3 coords]
        {
            get => GetChunkByPosition(coords)[coords];
            set => GetChunkByPosition(coords)[coords] = value;
        }

        public IEnumerable<Voxel> voxels => chunks.Select(kv => kv.Value)
            .Select(c => c.voxels)
            .SelectMany(v => v);

        private Chunk GetChunkByPosition(int3 pos)
        {
            var id = PositionToChunkId(pos);
            if (!chunks.ContainsKey(id))
            {
                chunks[id] = new(id);
            }

            return chunks[id];
        }

        private static int2 PositionToChunkId(int3 pos)
        {
            return new((pos.x - pos.x.Mod(Chunk.X_SIZE)) / Chunk.X_SIZE, (pos.z - pos.z.Mod(Chunk.Z_SIZE)) / Chunk.Z_SIZE);
        }
    }
}