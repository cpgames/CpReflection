using System.Collections.Generic;

namespace cpGames.core.CpReflection.Tests
{
    public class TestClass { }

    public class TestClassDerivedA : TestClass { }

    public class TestClassDerivedB : TestClass { }

    public class TestClassDerivedC : TestClassDerivedA { }

    public abstract class TestClassDerivedD : TestClass { }

    public struct TestStruct { }

    public class TestContainer : List<int> { }

    public enum TestEnum { }
}