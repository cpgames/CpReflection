using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cpGames.core.CpReflection.Tests
{
    [TestClass]
    public class Tests
    {
        #region Methods
        [TestMethod]
        public void GetElementType_Test()
        {
            var elementType = typeof(TestContainer).GetElementTypeEx();
            Assert.IsTrue(elementType == typeof(int));
            elementType = typeof(TestClass).GetElementTypeEx();
            Assert.IsNull(elementType);
            elementType = typeof(List<int>).GetElementTypeEx();
            Assert.IsTrue(elementType == typeof(int));
            elementType = typeof(int[]).GetElementTypeEx();
            Assert.IsTrue(elementType == typeof(int));
            elementType = typeof(Dictionary<int, string>).GetElementTypeEx();
            Assert.IsTrue(elementType == typeof(int));
            elementType = typeof(Dictionary<string, int>).GetElementTypeEx();
            Assert.IsTrue(elementType == typeof(string));
        }

        [TestMethod]
        public void IsStruct_Test()
        {
            var isStruct = typeof(TestStruct).IsStruct();
            Assert.IsTrue(isStruct);
            isStruct = typeof(TestClass).IsStruct();
            Assert.IsFalse(isStruct);
            isStruct = typeof(int).IsStruct();
            Assert.IsFalse(isStruct);
            isStruct = typeof(string).IsStruct();
            Assert.IsFalse(isStruct);
            isStruct = typeof(TestEnum).IsStruct();
            Assert.IsFalse(isStruct);
        }

        [TestMethod]
        public void FindAllRelatedTypes_Test()
        {
            var relatedTypes = typeof(TestClass).FindAllDerivedTypes().ToList();
            Assert.IsTrue(relatedTypes.Count == 3);
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedA)));
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedB)));
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedC)));

            relatedTypes = typeof(TestClass).FindAllDerivedTypes(null, true).ToList();
            Assert.IsTrue(relatedTypes.Count == 4);
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClass)));
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedA)));
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedB)));
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedC)));

            relatedTypes = typeof(TestClass).FindAllDerivedTypes(null, true, true).ToList();
            Assert.IsTrue(relatedTypes.Count == 5);
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClass)));
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedA)));
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedB)));
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedC)));
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedD)));

            relatedTypes = ReflectionCommon.FindAllDerivedTypes<TestClass>().ToList();
            Assert.IsTrue(relatedTypes.Count == 3);
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedA)));
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedB)));
            Assert.IsTrue(relatedTypes.Contains(typeof(TestClassDerivedC)));

            relatedTypes = ReflectionCommon.FindAllDerivedTypes<ITestInterface>().ToList();
            Assert.IsTrue(relatedTypes.Count == 0);
            relatedTypes = ReflectionCommon.FindAllDerivedTypes<ITestInterface>(null, true).ToList();
            Assert.IsTrue(relatedTypes.Count == 0);
            relatedTypes = ReflectionCommon.FindAllDerivedTypes<ITestInterface>(null, true, true).ToList();
            Assert.IsTrue(relatedTypes.Count == 2);
            Assert.IsTrue(relatedTypes.Contains(typeof(ITestInterface)));
            Assert.IsTrue(relatedTypes.Contains(typeof(ITestInterfaceDerived)));
        }

        [TestMethod]
        public void IsTypeOrDerived_Test()
        {
            var testClass = new TestClass();
            var testClassA = new TestClassDerivedA();
            var testClassB = new TestClassDerivedB();

            var isTypeOrDerived = typeof(TestClass).IsTypeOrDerived(typeof(TestClass));
            Assert.IsTrue(isTypeOrDerived);
            isTypeOrDerived = typeof(TestClassDerivedA).IsTypeOrDerived(typeof(TestClass));
            Assert.IsTrue(isTypeOrDerived);
            isTypeOrDerived = typeof(TestClassDerivedC).IsTypeOrDerived(typeof(TestClass));
            Assert.IsTrue(isTypeOrDerived);
            isTypeOrDerived = typeof(TestClassDerivedB).IsTypeOrDerived(typeof(TestClassDerivedA));
            Assert.IsFalse(isTypeOrDerived);
            isTypeOrDerived = typeof(TestClass).IsTypeOrDerived(typeof(TestClassDerivedA));
            Assert.IsFalse(isTypeOrDerived);

            isTypeOrDerived = ReflectionCommon.IsTypeOrDerived(testClass, testClass);
            Assert.IsTrue(isTypeOrDerived);
            isTypeOrDerived = ReflectionCommon.IsTypeOrDerived(testClass, testClassA);
            Assert.IsTrue(isTypeOrDerived);
            isTypeOrDerived = ReflectionCommon.IsTypeOrDerived(testClassA, testClass);
            Assert.IsFalse(isTypeOrDerived);
            isTypeOrDerived = ReflectionCommon.IsTypeOrDerived(testClassB, testClassA);
            Assert.IsFalse(isTypeOrDerived);

            isTypeOrDerived = ReflectionCommon.IsTypeOrDerived(typeof(TestClass), testClass);
            Assert.IsTrue(isTypeOrDerived);
            isTypeOrDerived = ReflectionCommon.IsTypeOrDerived(typeof(TestClass), testClassA);
            Assert.IsTrue(isTypeOrDerived);
            isTypeOrDerived = ReflectionCommon.IsTypeOrDerived(typeof(TestClassDerivedA), testClass);
            Assert.IsFalse(isTypeOrDerived);
            isTypeOrDerived = ReflectionCommon.IsTypeOrDerived(typeof(TestClassDerivedB), testClassA);
            Assert.IsFalse(isTypeOrDerived);
        }

        [TestMethod]
        public void InvokeMethod_Test()
        {
            var n = 5;
            var s = "test";
            var c = string.Format("{0}{1}", s, n);
            var testClass = new TestClass();

            // generic static
            var resultA = (int)ReflectionCommon.InvokeGenericStaticMethod(typeof(TestClass), "TestStaticGenericMethodA", n, typeof(int));
            Assert.IsTrue(resultA == n);
            resultA = (int)ReflectionCommon.InvokeGenericStaticMethod<TestClass>("TestStaticGenericMethodA", n, typeof(int));
            Assert.IsTrue(resultA == n);
            var resultB = (string)ReflectionCommon.InvokeGenericStaticMethod(typeof(TestClass), "TestStaticGenericMethodB", new object[] { s, n }, typeof(string), typeof(int));
            Assert.IsTrue(resultB == c);
            resultB = (string)ReflectionCommon.InvokeGenericStaticMethod<TestClass>("TestStaticGenericMethodB", new object[] { s, n }, typeof(string), typeof(int));
            Assert.IsTrue(resultB == c);

            // generic instance
            resultA = (int)ReflectionCommon.InvokeGenericMethod(testClass, "TestGenericMethodA", n, typeof(int));
            Assert.IsTrue(resultA == n);
            resultB = (string)ReflectionCommon.InvokeGenericMethod(testClass, "TestGenericMethodB", new object[] { s, n }, typeof(string), typeof(int));
            Assert.IsTrue(resultB == c);

            // static non-generic
            resultA = (int)ReflectionCommon.InvokeStaticMethod(typeof(TestClass), "TestStaticMethodA", n);
            Assert.IsTrue(resultA == n);
            resultA = (int)ReflectionCommon.InvokeStaticMethod<TestClass>("TestStaticMethodA", n);
            Assert.IsTrue(resultA == n);
            resultB = (string)ReflectionCommon.InvokeStaticMethod(typeof(TestClass), "TestStaticMethodB", new object[] { s, n });
            Assert.IsTrue(resultB == c);
            resultB = (string)ReflectionCommon.InvokeStaticMethod<TestClass>("TestStaticMethodB", new object[] { s, n });
            Assert.IsTrue(resultB == c);

            // instance non-generic
            resultA = (int)ReflectionCommon.InvokeMethod(testClass, "TestMethodA", n);
            Assert.IsTrue(resultA == n);
            resultB = (string)ReflectionCommon.InvokeMethod(testClass, "TestMethodB", new object[] { s, n });
            Assert.IsTrue(resultB == c);
        }

        [TestMethod]
        public void GetAttributes_Test()
        {
            var attA = typeof(TestClass).GetAttribute<TestClassAttributeA>();
            Assert.IsNotNull(attA);
            var attB = typeof(TestClass).GetAttribute<TestClassAttributeB>();
            Assert.IsNull(attB);
            attA = typeof(TestClassDerivedA).GetAttribute<TestClassAttributeA>();
            Assert.IsNotNull(attA);
            attB = typeof(TestClassDerivedA).GetAttribute<TestClassAttributeB>();
            Assert.IsNotNull(attB);

            var attsA = typeof(TestClass).GetAttributes<TestClassAttributeA>();
            Assert.IsTrue(attsA.Count() == 1);
            var attsAA = typeof(TestClass).GetAttributes<TestClassDerivedAttributeA>();
            Assert.IsTrue(attsAA.Count() == 1);
            attsA = typeof(TestClassDerivedA).GetAttributes<TestClassAttributeA>();
            Assert.IsTrue(attsA.Count() == 2);
        }

        [TestMethod]
        public void GenericArguments_Test()
        {
            var res = typeof(TestClassCDerived).HasGenericArgument(typeof(int)) &&
                typeof(TestClassCDerived).HasGenericArgument(typeof(string)) &&
                typeof(TestClassCDerived).HasGenericArgument(typeof(bool));
            Assert.IsTrue(res);
            res = typeof(TestClassCDerived).HasGenericArgument(typeof(long));
            Assert.IsFalse(res);
            var args = typeof(TestClassCDerived).GetGenericArgumentsEx();
            Assert.IsTrue(args.Length == 3);
            Assert.IsTrue(args.Contains(typeof(int)));
            Assert.IsTrue(args.Contains(typeof(string)));
            Assert.IsTrue(args.Contains(typeof(bool)));
        }
        #endregion
    }
}