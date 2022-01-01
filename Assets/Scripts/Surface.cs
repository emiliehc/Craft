using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace dev.hongjun.mc
{
    public enum CubeFace : byte
    {
        LEFT = 0,
        RIGHT = 1,
        BOTTOM = 2,
        TOP = 3,
        BACK = 4,
        FRONT = 5
    }

    public static class CubeSurfaceExt
    {
        private static readonly Vector3[][] cube =
        {
            // -x (left)
            new Vector3[]
            {
                new(-0.5f, -0.5f, 0.5f),
                new(-0.5f, 0.5f, 0.5f),
                new(-0.5f, 0.5f, -0.5f),
                new(-0.5f, -0.5f, -0.5f),
            },
            // +x (right)
            new Vector3[]
            {
                new(0.5f, -0.5f, -0.5f),
                new(0.5f, 0.5f, -0.5f),
                new(0.5f, 0.5f, 0.5f),
                new(0.5f, -0.5f, 0.5f),
            },
            // -y (bottom)
            new Vector3[]
            {
                new(-0.5f, -0.5f, 0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, 0.5f),
            },
            // +y (top)
            new Vector3[]
            {
                new(0.5f, 0.5f, 0.5f),
                new(0.5f, 0.5f, -0.5f),
                new(-0.5f, 0.5f, -0.5f),
                new(-0.5f, 0.5f, 0.5f),
            },
            // -z (back)
            new Vector3[]
            {
                new(-0.5f, -0.5f, -0.5f),
                new(-0.5f, 0.5f, -0.5f),
                new(0.5f, 0.5f, -0.5f),
                new(0.5f, -0.5f, -0.5f),
            },
            // +z (front)
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