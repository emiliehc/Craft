using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dev.hongjun.mc
{
    [SerializeMonoBehaviour]
    public class TestClass : MonoBehaviour
    {
        [SerializeField]
        [SerializeMonoBehaviourField] private int privateField;

        [SerializeField]
        [SerializeMonoBehaviourField] public Vector3 publicField;
    }
}