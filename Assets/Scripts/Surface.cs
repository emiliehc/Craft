using Unity.VisualScripting;
using UnityEngine;

namespace dev.hongjun.mc
{
    public enum CubeSurface
    {
        FRONT, TOP, RIGHT, LEFT, BACK, BOTTOM
    }
    
    public struct Surface
    {
        public Vector3Int position;
        public CubeSurface cubeSurface;
        public SurfaceTexture texture;
    }
}