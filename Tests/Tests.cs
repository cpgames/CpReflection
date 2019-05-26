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
            var elementType = ReflectionCommon.GetElementType(typeof(TestContainer));
            Assert.IsTrue(elementType == typeof(int));
            elementType = ReflectionCommon.GetElementType(typeof(TestClass));
            Assert.IsNull(elementType);
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
        }
        #endregion
    }
}