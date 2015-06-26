#region # using *.*

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LabySystem;
using LabySystem.Core;

#endregion

namespace LabyTest
{
  [TestClass]
  public class LabyGameTests
  {
    [TestMethod]
    public void TestLevelSizes()
    {
      int[] x = { 7, 7, 15, 23, 31, 47, 63, 95, 127, 191, 255, 383, 511, 767, 1023 };
      int[] y = { 5, 5, 9, 15, 19, 29, 39, 59, 79, 119, 159, 239, 319, 479, 639 };

      for (int level = 0; level < x.Length; level++)
      {
        var size = LabyGame.GetLevelSize(level);
        Assert.AreEqual(x[level], size.Item1);
        Assert.AreEqual(y[level], size.Item2);
      }
    }
  }
}
