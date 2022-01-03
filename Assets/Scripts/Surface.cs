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
        private static readonly float4[][] cube =
        {
            // -x (left)
            new float4[]
            {
                new(-0.5f, -0.5f, 0.5f, 1.0f),
                new(-0.5f, 0.5f, 0.5f, 1.0f),
                new(-0.5f, 0.5f, -0.5f, 1.0f),
                new(-0.5f, -0.5f, -0.5f, 1.0f),
            },
            // +x (right)
            new float4[]
            {
                new(0.5f, -0.5f, -0.5f, 1.0f),
                new(0.5f, 0.5f, -0.5f, 1.0f),
                new(0.5f, 0.5f, 0.5f, 1.0f),
                new(0.5f, -0.5f, 0.5f, 1.0f),
            },
            // -y (bottom)
            new float4[]
            {
                new(-0.5f, -0.5f, 0.5f, 1.0f),
                new(-0.5f, -0.5f, -0.5f, 1.0f),
                new(0.5f, -0.5f, -0.5f, 1.0f),
                new(0.5f, -0.5f, 0.5f, 1.0f),
            },
            // +y (top)
            new float4[]
            {
                new(0.5f, 0.5f, 0.5f, 1.0f),
                new(0.5f, 0.5f, -0.5f, 1.0f),
                new(-0.5f, 0.5f, -0.5f, 1.0f),
                new(-0.5f, 0.5f, 0.5f, 1.0f),
            },
            // -z (back)
            new float4[]
            {
                new(-0.5f, -0.5f, -0.5f, 1.0f),
                new(-0.5f, 0.5f, -0.5f, 1.0f),
                new(0.5f, 0.5f, -0.5f, 1.0f),
                new(0.5f, -0.5f, -0.5f, 1.0f),
            },
            // +z (front)
            new float4[]
            {
                new(0.5f, -0.5f, 0.5f, 1.0f),
                new(0.5f, 0.5f, 0.5f, 1.0f),
                new(-0.5f, 0.5f, 0.5f, 1.0f),
                new(-0.5f, -0.5f, 0.5f, 1.0f),
            }
        };

        private static readonly int3[] directions =
        {
            (int3) left(),
            (int3) right(),
            (int3) down(),
            (int3) up(),
            (int3) back(),
            (int3) forward(),
        };

        public static float4[] GetUnitVertices(this CubeFace face)
        {
            return cube[(int) face];
        }

        public static CubeFace Opposite(this CubeFace face)
        {
            return (byte) face % 2 == 0 ? face + 1 : face - 1;
        }

        public static int3 GetDirectionalVector(this CubeFace face)
        {
            return directions[(int) face];
        }
    }

    public struct Surface
    {
        public int3 position;
        public CubeFace face;
        public SurfaceTexture texture;
        public float light;
    }
}