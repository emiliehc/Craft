using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace dev.hongjun.mc
{
    public class DataStructureTest
    {

        [Test]
        public void RelativeCoordinatesTest()
        {
            var s = new RelativeCoordinates(1, 2, 3);
            
            Assert.AreEqual(1, s.x);
            Assert.AreEqual(2, s.y);
            Assert.AreEqual(3, s.z);

            for (var i = 0; i < 1000; i++)
            {
                var x = random(16);
                var y = random(256);
                var z = random(16);
                s = new(x, y, z);
                Assert.AreEqual(x, s.x);
                Assert.AreEqual(y, s.y);
                Assert.AreEqual(z, s.z);
                
                x = random(16);
                y = random(256);
                z = random(16);
                s.x = x;
                s.y = y;
                s.z = z;
                
                Assert.AreEqual(x, s.x);
                Assert.AreEqual(y, s.y);
                Assert.AreEqual(z, s.z);
            }
        }

        private static byte random(int range)
        {
            return (byte)Random.Range(0, range);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator DataStructureTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}

