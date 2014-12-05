using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Penates.Utils;

namespace Penates.Tests.Utils {
    [TestClass]
    public class ListUtilsTest {
        [TestMethod]
        public void interlayTestSameItems() {
            List<int> a = new List<int>() { 1, 3, 5, 7, 9 };
            List<int> b = new List<int>() { 2, 4, 6, 8, 10 };

            a = ListUtils<int>.interlay(a, b);

            List<int> expected = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            CollectionAssert.AreEqual(expected, a);
        }

        [TestMethod]
        public void interlayTestDif() {
            List<int> a = new List<int>() { 1, 3, 5, 7, 9};
            List<int> b = new List<int>() { 2, 4, 6, 8};

            a = ListUtils<int>.interlay(a, b);

            List<int> expected = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9};

            CollectionAssert.AreEqual(expected, a);
        }
    }
}
