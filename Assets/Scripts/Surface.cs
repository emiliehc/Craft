using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace dev.hongjun.mc
{
    public enum CubeFace : byte
    {
        BACK = 0,
        FRONT = 1,
        BOTTOM = 2,
        TOP = 3,
        LEFT = 4,
        RIGHT = 5
    }

    public static class CubeSurfaceExt
    {
        private static readonly Vector3[][] cube =
        {
            new Vector3[]
            {
                new(-0.5f, -0.5f, 0.5f),
                new(-0.5f, 0.5f, 0.5f),
                new(-0.5f, 0.5f, -0.5f),
                new(-0.5f, -0.5f, -0.5f),
            },
            new Vector3[]
            {
                new(0.5f, -0.5f, -0.5f),
                new(0.5f, 0.5f, -0.5f),
                new(0.5f, 0.5f, 0.5f),
                new(0.5f, -0.5f, 0.5f),
            },
            new Vector3[]
            {
                new(-0.5f, -0.5f, 0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, 0.5f),
            },
            new Vector3[]
            {
                new(0.5f, 0.5f, 0.5f),
                new(0.5f, 0.5f, -0.5f),
                new(-0.5f, 0.5f, -0.5f),
                new(-0.5f, 0.5f, 0.5f),
            },
            new Vector3[]
            {
                new(-0.5f, -0.5f, -0.5f),
                new(-0.5f, 0.5f, -0.5f),
                new(0.5f, 0.5f, -0.5f),
                new(0.5f, -0.5f, -0.5f),
            },
            new Vector3[]
            {
                new(0.5f, -0.5f, 0.5f),
                new(0.5f, 0.5f, 0.5f),
                new(-0.5f, 0.5f, 0.5f),
                new(-0.5f, -0.5f, 0.5f),
            }
        };

        public static Vector3[] GetUnitVertices(this CubeFace face)
        {
            return cube[(int) face];
        }
    }

    public struct Surface
    {
        public int3 position;
        public CubeFace face;
        public SurfaceTexture texture;
    }
}