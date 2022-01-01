using UnityEngine;

namespace dev.hongjun.mc
{
    public struct Voxel
    {
        public Vector3Int position;
        public SurfaceTexture texId;

        public Voxel(in Vector3Int pos, SurfaceTexture texture = SurfaceTexture.STONE)
        {
            position = pos;
            texId = texture;
        }

        public Voxel(in Vector3Int pos, int texture) : this(pos, (SurfaceTexture) texture)
        {
        }

        public struct Face
        {
            
        }
    }
}