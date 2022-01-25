using System;
using UnityEngine;

namespace dev.hongjun.mc
{
    public class SerializationManagerTestDriver : MonoBehaviour
    {
        private void Start()
        {
            Invoke(nameof(Test), 2.0f);
        }

        private void Test()
        {
            var snapshot = SerializationManager.Instance.TakeSnapshot();
        }
    }
}