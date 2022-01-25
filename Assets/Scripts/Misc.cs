using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Unity.Mathematics.math;
using System.Text;
using JetBrains.Annotations;
using Mono.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace dev.hongjun.mc
{
    public abstract class Singleton<T> : Singleton where T : MonoBehaviour
    {
        [CanBeNull] private static T _instance;

        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new();

        [SerializeField] private bool _persistent = true;

        [NotNull]
        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    Debug.LogWarning(
                        $"[{typeof(Singleton)}<{typeof(T)}>] Instance will not be returned because the application is quitting.");
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return null;
                }

                lock (Lock)
                {
                    if (_instance != null)
                        return _instance;
                    var instances = FindObjectsOfType<T>();
                    var count = instances.Length;
                    if (count > 0)
                    {
                        if (count == 1)
                            return _instance = instances[0];
                        Debug.LogWarning(
                            $"[{typeof(Singleton)}<{typeof(T)}>] There should never be more than one {typeof(Singleton)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                        for (var i = 1; i < instances.Length; i++)
                            Destroy(instances[i]);
                        return _instance = instances[0];
                    }

                    Debug.Log(
                        $"[{typeof(Singleton)}<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                    return _instance = new GameObject($"({typeof(Singleton)}){typeof(T)}")
                        .AddComponent<T>();
                }
            }
        }

        private void Awake()
        {
            if (_persistent)
                DontDestroyOnLoad(gameObject);
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }
    }

    public abstract class Singleton : MonoBehaviour
    {
        public static bool Quitting { get; private set; }

        private void OnApplicationQuit()
        {
            Quitting = true;
        }
    }

    public static class Utility
    {
        #region string

        public static string ToSnakeCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var builder = new StringBuilder(text.Length + Math.Min(2, text.Length / 5));
            var previousCategory = default(UnicodeCategory?);

            for (var currentIndex = 0; currentIndex < text.Length; currentIndex++)
            {
                var currentChar = text[currentIndex];
                if (currentChar == '_')
                {
                    builder.Append('_');
                    previousCategory = null;
                    continue;
                }

                var currentCategory = char.GetUnicodeCategory(currentChar);
                switch (currentCategory)
                {
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.TitlecaseLetter:
                        if (previousCategory == UnicodeCategory.SpaceSeparator ||
                            previousCategory == UnicodeCategory.LowercaseLetter ||
                            previousCategory != UnicodeCategory.DecimalDigitNumber &&
                            previousCategory != null &&
                            currentIndex > 0 &&
                            currentIndex + 1 < text.Length &&
                            char.IsLower(text[currentIndex + 1]))
                        {
                            builder.Append('_');
                        }

                        currentChar = char.ToLower(currentChar, CultureInfo.InvariantCulture);
                        break;

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (previousCategory == UnicodeCategory.SpaceSeparator)
                        {
                            builder.Append('_');
                        }

                        break;

                    default:
                        if (previousCategory != null)
                        {
                            previousCategory = UnicodeCategory.SpaceSeparator;
                        }

                        continue;
                }

                builder.Append(currentChar);
                previousCategory = currentCategory;
            }

            return builder.ToString();
        }

        #endregion

        #region Texture

        public static Texture2D ResizeTo(this Texture2D texture2D, int targetX, int targetY,
            TextureFormat? format = null)
        {
            texture2D.filterMode = FilterMode.Point;
            var rt = new RenderTexture(targetX, targetY, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            var result = new Texture2D(targetX, targetY, format ?? texture2D.format, false)
            {
                filterMode = FilterMode.Point,
                name = texture2D.name
            };
            result.ReadPixels(new(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            return result;
        }

        #endregion
    }

    public static class MathExt
    {
        public static int Mod(this int x, int m)
        {
            return (x % m + m) % m;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 ToPoint(this int3 v)
        {
            return new(v.x, v.y, v.z, 1.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetLowNibble(this byte input)
        {
            return (byte) (input & 0x0F);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetHighNibble(this byte input)
        {
            return (byte) (input & 0xF0);
        }
    }

    public interface IFlattenable<out T>
    {
        T[] Flatten();
    }


    public readonly unsafe struct FlatArray2U<T> : IEnumerable<T> where T : unmanaged
    {
        private readonly T* data;

        public int l0 { get; }

        public int l1 { get; }

        public FlatArray2U(T* data, int l0, int l1)
        {
            this.data = data;
            this.l0 = l0;
            this.l1 = l1;
        }

        public T this[int x, int y]
        {
            get => data[y * l0 + x];
            set => data[y * l0 + x] = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var y = 0; y < l1; y++)
            {
                for (var x = 0; x < l0; x++)
                {
                    yield return this[x, y];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class FlatArray3M<T> : ICollection<T>, IFlattenable<T>
    {
        private T[,,] data;

        public int l0 { get; }
        public int l1 { get; }
        public int l2 { get; }

        public FlatArray3M(int l0, int l1, int l2)
        {
            this.l0 = l0;
            this.l1 = l1;
            this.l2 = l2;

            data = new T[l0, l1, l2];
        }

        public T this[int x, int y, int z]
        {
            get => data[x, y, z];
            set => data[x, y, z] = value;
        }

        public T this[int3 index]
        {
            get => this[index.x, index.y, index.z];
            set => this[index.x, index.y, index.z] = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var z = 0; z < l2; z++)
            {
                for (var y = 0; y < l1; y++)
                {
                    for (var x = 0; x < l0; x++)
                    {
                        yield return this[x, y, z];
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            data = new T[l0, l1, l2];
        }

        public bool Contains(T item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var src = Flatten();
            Array.Copy(src, 0, array, arrayIndex, Count);
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public int Count => l0 * l1 * l2;

        public bool IsReadOnly => false;

        public T[] Flatten()
        {
            return this.ToArray();
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = sizeof(ushort))]
    public struct RelativeCoordinates
    {
        [FieldOffset(0)] public byte y;
        [FieldOffset(1)] private byte xz;

        public byte x
        {
            get => xz.GetLowNibble();
            set => xz = (byte) (xz & 0x0F + value);
        }

        public byte z
        {
            get => xz.GetHighNibble();
            set => xz = (byte) (xz & 0xF0 + value << 4);
        }

        public RelativeCoordinates(byte x, byte y, byte z)
        {
            this.y = y;
            xz = (byte) (((z << 4) & 0xF0) | (x & 0xF0));
        }

        public static unsafe explicit operator ushort(RelativeCoordinates coords)
        {
            return *(ushort*) &coords;
        }
    }
}