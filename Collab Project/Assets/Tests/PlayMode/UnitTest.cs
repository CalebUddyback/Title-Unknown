using System.Collections;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class UnitTest
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator Punch()
        {
            GameObject gameObject = new GameObject("test", typeof(Sakura));

            Assert.AreEqual(expected: "Punch", actual: gameObject.GetComponent<Combat_Character>().attackList[0].name);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Uppercut()
        {
            GameObject gameObject = new GameObject("test", typeof(Sakura));

            Assert.AreEqual(expected: "Uppercut", actual: gameObject.GetComponent<Combat_Character>().attackList[1].name);

            yield return null;
        }
    }
}
