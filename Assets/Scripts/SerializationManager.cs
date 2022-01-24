using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace dev.hongjun.mc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializeMonoBehaviourAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeMonoBehaviourFieldAttribute : Attribute
    {
        public bool ByReference = true;
    }

    [SerializeMonoBehaviour]
    public class TestClass : MonoBehaviour
    {
        [SerializeMonoBehaviourField]
        private int privateField;

        [SerializeMonoBehaviourField] 
        public Vector3 publicField;
    }

    public class Snapshot
    {
        public struct Transform
        {
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;
        }

        public class ObjectSnapshot
        {
            private readonly Dictionary<Type, List<(FieldInfo, object)>> snapshots;

            public void AddField(Type type, FieldInfo field, object value)
            {
                if (!snapshots.ContainsKey(type))
                    snapshots[type] = new();
                
                snapshots[type].Add((field, value));
            }
        }

        private readonly Dictionary<int, ObjectSnapshot> snapshots = new();
    }
    
    public class SerializationManager : Singleton<SerializationManager>
    {
        private Dictionary<Type, List<FieldInfo>> toSerialize;

        private void Start()
        {
            // get all classes to serialize
            var serializableTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(assembly => assembly.GetTypes())
                .SelectMany(t => t)
                .Where(t => t.IsDefined(typeof(SerializeMonoBehaviourAttribute), false))
                .Where(t => t.IsSubclassOf(typeof(MonoBehaviour)))
                .Select(t => (t, t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(f => f.IsDefined(typeof(SerializeMonoBehaviourFieldAttribute), false))
                    .ToList()));

            foreach (var (key, value) in serializableTypes)
            {
                toSerialize[key] = value;
            }
            
        }

        public Snapshot TakeSnapshot()
        {
            Snapshot snapshot = new();
            
            var objects = GameObject.FindGameObjectsWithTag("Checkpoint");
            foreach (var obj in objects)
            {
                var id = obj.GetInstanceID();
                //obj.GetComponent<>()
            }

            return null;
        }

        /// <summary>
        /// Does not revive dead objects !
        /// </summary>
        public void RestoreSnapshot(Snapshot snapshot)
        {
            
        }
    }
}