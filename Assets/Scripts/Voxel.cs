using Unity.Mathematics;
using UnityEngine;

namespace dev.hongjun.mc
{
    public struct Voxel
    {
        public int3 position;
        public SurfaceTexture texId;

        public Voxel(in int3 pos, SurfaceTexture texture = SurfaceTexture.STONE)
        {
            position = pos;
            texId = texture;
        }

        public Voxel(in int3 pos, int texture) : this(pos, (SurfaceTexture) texture)
        {
        }
    }
}