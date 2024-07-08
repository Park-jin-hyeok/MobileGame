using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

class AdaptivePerformanceGoogleAndroidTests
{

    [UnityTest]
    public IEnumerator DummyAPGoogleAndroidTest()
    {
        yield return null;
        Assert.AreEqual(1, 1);
    }
}
