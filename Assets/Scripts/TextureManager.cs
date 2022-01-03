using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace dev.hongjun.mc
{
    public enum SurfaceTexture : ushort
    {
        STONE = 0,
        DIRT = 1,
        GRASS = 2,
        IRON_ORE = 3,
        OAK_WOOD_PLANK = 4,
        DIAMOND_BLOCK = 5
    }

    public static class SurfaceTextureExt
    {
        public static float2[] GetUv(this SurfaceTexture texture)
        {
            return TextureManager.Instance.GetUv(texture);
        }
    }

    public class TextureManager : Singleton<TextureManager>
    {
        private Dictionary<SurfaceTexture, float2[]> surTexToUv;
        private Dictionary<SurfaceTexture, Texture2D> surfTexToTex;
        public Texture2D masterTexture { get; private set; }
        
#if UNITY_EDITOR
        private int textureCount;
        [HideInInspector]
        public bool masterTexDebugObjShown = false;
#endif

        private unsafe void Awake()
        {
            surTexToUv = new();
            surfTexToTex = new();
            var textures = Resources.LoadAll("Textures", typeof(Texture2D))
                .Select(t => t as Texture2D)
                .Where(t => t is not null)
                .Select(t => t.ResizeTo(16, 16, TextureFormat.RGBAFloat))
                .ToList();

            masterTexture = new(16 * textures.Count, 16, TextureFormat.RGBAFloat, false);
            using var arr = masterTexture.GetPixelData<Color>(0);

            var masterTexturePtr = (Color*) arr.GetUnsafePtr();
            var masterTextureArr = new FlatArray2U<Color>(masterTexturePtr, 16 * textures.Count, 16);

            Debug.Log(arr.Length);

            for (var i = 0; i < textures.Count; i++)
            {
                var t = textures[i];
                using var data = t.GetPixelData<Color>(0);

                var dataPtr = (Color*) data.GetUnsafePtr();
                var dataArr = new FlatArray2U<Color>(dataPtr, 16, 16);
                for (var x = 0; x < 16; x++)
                {
                    for (var y = 0; y < 16; y++)
                    {
                        masterTextureArr[x + i * 16, y] = dataArr[x, y];
                    }
                }
            }

            masterTexture.filterMode = FilterMode.Point;
            masterTexture.SetPixelData(arr, 0);
            masterTexture.Apply();

            // set up uv map
            var texCount = textures.Count;
            var singleTexWidth = 1.0f / texCount;
            for (var i = 0; i < texCount; i++)
            {
                var t = textures[i];
                var texName = t.name.ToSnakeCase().ToUpper();
                Debug.Log($"Loaded texture {texName}");
                Assert.IsTrue(Enum.TryParse(texName, out SurfaceTexture surfaceTex));
                var offset = singleTexWidth * i;
                surTexToUv[surfaceTex] = new[]
                {
                    new float2(offset + 0.0f, 0.0f),
                    new float2(offset + 0.0f, 1.0f),
                    new float2(offset + singleTexWidth, 1.0f),
                    new float2(offset + singleTexWidth, 0.0f),
                };
                surfTexToTex[surfaceTex] = t;
            }
            
#if UNITY_EDITOR
            textureCount = texCount;
#endif
        }

        public float2[] GetUv(SurfaceTexture tex)
        {
            return surTexToUv[tex];
        }

        public Texture2D GetTexture(SurfaceTexture tex)
        {
            return surfTexToTex[tex];
        }

#if UNITY_EDITOR
        public void GenerateMasterTextureDebugObject()
        {
            var debugObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            debugObj.name = "Master Texture Debug";
            debugObj.GetComponent<Renderer>().material.mainTexture = masterTexture;
            debugObj.transform.localScale = new(textureCount, 1.0f, 1.0f);
        }
#endif
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(TextureManager))]
    public class TextureManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!TextureManager.Instance.masterTexDebugObjShown &&
                GUI.Button(GUILayoutUtility.GetLastRect(), "Show Master Texture Debug Object"))
            {
                TextureManager.Instance.masterTexDebugObjShown = true;
                TextureManager.Instance.GenerateMasterTextureDebugObject();
            }
        }
    }

#endif
}