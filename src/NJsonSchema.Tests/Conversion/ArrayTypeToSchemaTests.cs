﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace NJsonSchema.Tests.Conversion
{
    [TestClass]
    public class ArrayTypeToSchemaTests
    {
        public class DictionarySubType : DictionaryType
        {

        }

        public class DictionaryType : Dictionary<string, IList<string>>
        {
            public string Foo { get; set; }
        }

        [TestMethod]
        public async Task When_converting_type_inheriting_from_dictionary_then_it_should_be_correct()
        {
            //// Act
            var dict = new DictionarySubType();
            dict.Foo = "abc";
            dict.Add("bar", new List<string> { "a", "b" });
            var json = JsonConvert.SerializeObject(dict);

            var schema = await JsonSchema4.FromTypeAsync<DictionarySubType>();
            var data = schema.ToJson();

            //// Assert
            Assert.AreEqual(JsonObjectType.Object, schema.Type);
            Assert.IsFalse(json.Contains("Foo"));
            Assert.IsFalse(json.Contains("foo"));
        }

        [TestMethod]
        public async Task When_converting_array_then_items_must_correctly_be_loaded()
        {
            await When_converting_array_then_items_must_correctly_be_loaded("Array");
        }

        [TestMethod]
        public async Task When_converting_collection_then_items_must_correctly_be_loaded()
        {
            await When_converting_array_then_items_must_correctly_be_loaded("Collection");
        }

        [TestMethod]
        public async Task When_converting_list_then_items_must_correctly_be_loaded()
        {
            await When_converting_array_then_items_must_correctly_be_loaded("List");
        }

        [TestMethod]
        public async Task When_converting_interface_list_then_items_must_correctly_be_loaded()
        {
            await When_converting_array_then_items_must_correctly_be_loaded("InterfaceList");
        }

        [TestMethod]
        public async Task When_converting_enumerable_list_then_items_must_correctly_be_loaded()
        {
            await When_converting_array_then_items_must_correctly_be_loaded("Enumerable");
        }

        public class MyType
        {
            public MySubtype Reference { get; set; }

            public MySubtype[] Array { get; set; }

            public Collection<MySubtype> Collection { get; set; }

            public List<MySubtype> List { get; set; }

            public IList<MySubtype> InterfaceList { get; set; }

            public IEnumerable<MySubtype> Enumerable { get; set; }
        }

        public class MySubtype
        {
            public string Id { get; set; }
        }

        public async Task When_converting_array_then_items_must_correctly_be_loaded(string propertyName)
        {
            //// Act
            var schema = await JsonSchema4.FromTypeAsync<MyType>();
            var schemaData = schema.ToJson();

            //// Assert
            var property = schema.Properties[propertyName];

            Assert.AreEqual(JsonObjectType.Array | JsonObjectType.Null, property.Type);
            Assert.AreEqual(JsonObjectType.Object, property.ActualSchema.Item.ActualSchema.Type);
            Assert.IsTrue(schema.Definitions.Any(d => d.Key == "MySubtype"));
            Assert.AreEqual(JsonObjectType.String | JsonObjectType.Null, property.ActualSchema.Item.ActualSchema.Properties["Id"].Type);
        }
    }
}
