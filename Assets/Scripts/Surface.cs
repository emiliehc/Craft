using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace dev.hongjun.mc
{
    public enum CubeSurface : byte
    {
        BACK = 0, FRONT = 1, BOTTOM = 2, TOP = 3, LEFT = 4, RIGHT = 5
    }
    
    public struct Surface
    {
        public int3 position;
        public CubeSurface cubeSurface;
        public SurfaceTexture texture;
    }
}