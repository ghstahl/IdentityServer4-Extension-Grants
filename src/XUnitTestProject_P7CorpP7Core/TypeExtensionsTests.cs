using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using P7Core.Reflection;
using P7Core.Writers;
using Shouldly;
using Xunit;

namespace XUnitTestProject_P7CorpP7Core
{
    public class CustomAttribute : System.Attribute
    {

    }
    public class CustomAttribute2 : System.Attribute
    {

    }
    public class SomeBaseClass { }
    [CustomAttribute]
    public class SomePublicClass: SomeBaseClass
    {
        public const string Dog = "dog";
        public const int Age = 1;
        public string Cat { get; set; }
   
    }
    class SomePrivateClass { }
    public abstract class SomeAbstractClass { }
    public interface  ISomePublicInterface { }
    public struct SomeStruct { }
    public enum SomeEnum { }
    public class TypeExtensionsTests
    {
        [Fact]
        void Test_FindDerivedTypes()
        {
            TypeHelper<JsonDocumentWriter>.FindDerivedTypes(typeof(SomePrivateClass).Assembly)
                .ToList().Count.ShouldBe(0);
            TypeHelper<SomeBaseClass>.FindDerivedTypes(typeof(SomeBaseClass).Assembly)
                .ToList().Count.ShouldBeGreaterThan(0);
        }
        [Fact]
        void Test_FindTypesInAssembly2()
        {
            TypeHelper<SomePublicClass>.FindTypesInAssembly2(typeof(SomeBaseClass).Assembly)
                .ToList().Count.ShouldBeGreaterThan(0);
        }
        [Fact]
        void Test_is_subclass()
        {
            TypeHelper<SomeEnum>.IsSubclassOf(null).ShouldBeFalse();
            TypeHelper<SomeBaseClass>.IsSubclassOf(null).ShouldBeFalse();
            TypeHelper<SomeBaseClass>.IsSubclassOf(typeof(SomePublicClass)).ShouldBeTrue();
            TypeHelper<SomePublicClass>.IsSubclassOf(typeof(SomeAbstractClass)).ShouldBeFalse();
        }
        [Fact]
        void Test_is_type()
        {
            TypeHelper<SomePublicClass>.IsType(typeof(SomePublicClass)).ShouldBeTrue();
            TypeHelper<string>.IsType(typeof(SomePublicClass)).ShouldBeFalse();
        }

        [Fact]
        void Test_FindTypesInAssembly()
        {
            var types = TypeHelper<Type>.FindTypesInAssembly(typeof(SomePublicClass).Assembly, t =>
            {
                return true;
            });
            types.Any().ShouldBeTrue();
            
        }
        [Fact]
        void Test_get_WithCustomAttribute_success()
        {
            List<Type> master = new List<Type>()
            {
                typeof(SomePublicClass)
            };
            var constants = master.WithCustomAttribute<CustomAttribute>();

            constants.ToList().Count.ShouldBeGreaterThan(0);

            TypeHelper<Type>.FindTypesWithCustomAttribute< CustomAttribute>(master).ToList().Count.ShouldBeGreaterThan(0);
        }
        [Fact]
        void Test_get_WithCustomAttribute_fail()
        {
            List<Type> master = new List<Type>()
            {
                typeof(SomePublicClass)
            };
            var constants = master.WithCustomAttribute<CustomAttribute2>();

            constants.ToList().Count.ShouldBe(0);
        }
        [Fact]
        void Test_get_constants_success()
        {
            var constants = typeof(SomePublicClass).GetConstants();
            constants.ToList().Count.ShouldBeGreaterThan(0);
        }
        [Fact]
        void Test_get_constant_values_string_success()
        {
            var constants = typeof(SomePublicClass).GetConstantsValues<string>();
            constants.ToList().Count.ShouldBeGreaterThan(0);
        }
        [Fact]
        void Test_get_constant_values_int_success()
        {
            var constants = typeof(SomePublicClass).GetConstantsValues<int>();
            constants.ToList().Count.ShouldBeGreaterThan(0);
        }
        [Fact]
        void Test_get_constant_values_SomePrivateClass_fail()
        {
            var constants = typeof(SomePublicClass).GetConstantsValues<SomePrivateClass>();
            constants.ToList().Count.ShouldBe(0);
        }
        [Fact]
        void Test_is_public_true()
        {
            TypeHelper<SomePublicClass>.IsPublicClassType().ShouldBeTrue();
            typeof(SomePublicClass).IsPublicClass().ShouldBeTrue();
        }

        [Fact]
        void Test_is_public_false_abstract()
        {
            typeof(SomeAbstractClass).IsPublicClass().ShouldBeFalse();
        }

        [Fact]
        void Test_is_public_false_private()
        {
            typeof(SomePrivateClass).IsPublicClass().ShouldBeFalse();
        }

        [Fact]
        void Test_is_public_false_notclass()
        {
            typeof(ISomePublicInterface).IsPublicClass().ShouldBeFalse();
            typeof(SomeAbstractClass).IsPublicClass().ShouldBeFalse();
            typeof(int).IsPublicClass().ShouldBeFalse();
            typeof(bool).IsPublicClass().ShouldBeFalse();
            typeof(SomeEnum).IsPublicClass().ShouldBeFalse();
        }

        [Fact]
        void Test_IsGenericList_true()
        {
            List<string> theList = new List<string>();
            theList.IsGenericList().ShouldBeTrue();
        }

        [Fact]
        void Test_IsGenericList_false()
        {
            "".IsGenericList().ShouldBeFalse();
        }

        [Fact]
        void Test_AssemblyNameWithoutVersion_success()
        {
            typeof(TypeExtensionsTests)
                .Assembly
                .AssemblyNameWithoutVersion()
                .ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        void Test_AssemblyQualifiedNameWithoutVersion_success()
        {
            typeof(TypeExtensionsTests)
                .AssemblyQualifiedNameWithoutVersion()
                .ShouldNotBeNullOrWhiteSpace();
        }
    }
}
