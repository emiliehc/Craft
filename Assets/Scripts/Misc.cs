using System;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;
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
                filterMode = FilterMode.Point
            };
            result.name = texture2D.name;
            result.ReadPixels(new(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            return result;
        }
        
        #endregion
    }


    public readonly unsafe struct Linear2DArrayAccessor<T> where T : unmanaged
    {
        private readonly T* data;
        private readonly int width;
        private readonly int height;

        public Linear2DArrayAccessor(T* data, int width, int height)
        {
            this.data = data;
            this.width = width;
            this.height = height;
        }

        public ref T this[int x, int y] => ref data[y * width + x];
    }
}