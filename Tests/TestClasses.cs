using System;
using System.Collections.Generic;

namespace cpGames.core.CpReflection.Tests
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TestClassAttributeA : Attribute { }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TestClassDerivedAttributeA : Attribute { }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TestClassAttributeB : Attribute { }

    [TestClassAttributeA]
    [TestClassDerivedAttributeA]
    public class TestClass
    {
        #region Methods
        public static T TestStaticGenericMethodA<T>(T data)
        {
            return data;
        }

        public static string TestStaticGenericMethodB<T, U>(T dataA, U dataB)
        {
            return string.Format("{0}{1}", dataA, dataB);
        }

        public T TestGenericMethodA<T>(T data)
        {
            return data;
        }

        public string TestGenericMethodB<T, U>(T dataA, U dataB)
        {
            return string.Format("{0}{1}", dataA, dataB);
        }

        public static int TestStaticMethodA(int data)
        {
            return data;
        }

        public static string TestStaticMethodB(string dataA, int dataB)
        {
            return string.Format("{0}{1}", dataA, dataB);
        }

        public int TestMethodA(int data)
        {
            return data;
        }

        public string TestMethodB(string dataA, int dataB)
        {
            return string.Format("{0}{1}", dataA, dataB);
        }
        #endregion
    }

    [TestClassAttributeA]
    [TestClassAttributeB]
    public class TestClassDerivedA : TestClass { }

    public class TestClassDerivedB : TestClass { }

    public class TestClassDerivedC : TestClassDerivedA { }

    public abstract class TestClassDerivedD : TestClass { }

    public interface ITestInterface { }

    public interface ITestInterfaceDerived : ITestInterface { }

    public struct TestStruct { }

    public class TestContainer : List<int> { }

    public enum TestEnum { }

    public class TestClassC<T, U, V> { }

    public class TestClassCDerived : TestClassC<int, string, bool> { }
}