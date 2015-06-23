#region # using *.*

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LabySystem;
using LabySystem.Core;

#endregion

namespace LabyTest
{
  [TestClass]
  public class LabyCoreTests
  {
    /// <summary>
    /// Core.Knot64 testen
    /// </summary>
    [TestMethod]
    public void TestKnot64()
    {
      Knot64 k1 = new Knot64(123, false, false);
      Assert.AreEqual(123, k1.WallNumber);
      Assert.IsFalse(k1.WallTop);
      Assert.IsFalse(k1.WallLeft);

      Knot64 k2 = new Knot64(123456, true, false);
      Assert.AreEqual(123456, k2.WallNumber);
      Assert.IsTrue(k2.WallTop);
      Assert.IsFalse(k2.WallLeft);

      Knot64 k3 = new Knot64(123456789, false, true);
      Assert.AreEqual(123456789, k3.WallNumber);
      Assert.IsFalse(k3.WallTop);
      Assert.IsTrue(k3.WallLeft);

      k1.WallNumber *= 3;
      k1.WallTop = !k1.WallTop;
      k1.WallLeft = !k1.WallLeft;
      Assert.AreEqual(123 * 3, k1.WallNumber);
      Assert.IsTrue(k1.WallTop);
      Assert.IsTrue(k1.WallLeft);

      k2.WallTop = !k2.WallTop;
      k2.WallLeft = !k2.WallLeft;
      Assert.AreEqual(123456, k2.WallNumber);
      Assert.IsFalse(k2.WallTop);
      Assert.IsTrue(k2.WallLeft);

      k3.WallTop = !k3.WallTop;
      k3.WallLeft = !k3.WallLeft;
      Assert.AreEqual(123456789, k3.WallNumber);
      Assert.IsTrue(k3.WallTop);
      Assert.IsFalse(k3.WallLeft);
    }
  }
}
