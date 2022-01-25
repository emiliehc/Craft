using System;
using UnityEngine;

namespace dev.hongjun.mc
{
    public class SerializationManagerTestDriver : MonoBehaviour
    {
        private Snapshot snapshot;
        
        private void Start()
        {
            InvokeRepeating(nameof(Test1), 5.0f, 20.0f);
            InvokeRepeating(nameof(Test2), 15.0f, 20.0f);
        }

        private void Test1()
        {
            snapshot = SerializationManager.Instance.TakeSnapshot();
            print("Snapshot taken");
        }

        private void Test2()
        {
            SerializationManager.Instance.RestoreSnapshot(snapshot);
            print("Snapshot restored");
        }
    }
}