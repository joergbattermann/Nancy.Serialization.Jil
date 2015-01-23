using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nancy.ModelBinding;

namespace Nancy.Serialization.Jil.Tests
{
    [TestClass]
    public class JilBodyDeserializerFixture
    {
        public class TestData
        {
            public TestData()
            {
            }
            
            public string SomeString { get; set; }

            public Guid SomeGuid { get; set; }
        }

        public class TestDataWithList
        {
            public TestDataWithList()
            {
            }
            
            public string SomeString { get; set; }

            public Guid SomeGuid { get; set; }

            public List<string> SomeList { get; set; }
        }

        [TestMethod]
        public void when_deserializing()
        {
            // Given
            var guid = Guid.NewGuid();
            string source = string.Format("{{\"SomeString\":\"some string value\",\"SomeGuid\":\"{0}\"}}", guid);

            var context = new BindingContext
            {
                DestinationType = typeof(TestData),
                ValidModelBindingMembers = typeof(TestData).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new BindingMemberInfo(p)),
            };

            // When
            object actual;
            using (var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(source)))
            {
                IBodyDeserializer jilBodyDeserializer = new JilBodyDeserializer();
                actual = jilBodyDeserializer.Deserialize("application/json", bodyStream, context);
            }

            // Then
            Assert.IsInstanceOfType(actual, typeof(TestData));

            var actualData = actual as TestData;
            Assert.IsNotNull(actualData);
            Assert.AreEqual("some string value", actualData.SomeString);
            Assert.AreEqual(guid, actualData.SomeGuid);
        }

        [TestMethod]
        public void when_deserializing_with_blacklisted_property()
        {
            // Given
            var guid = Guid.NewGuid();
            string source = string.Format("{{\"SomeString\":\"some string value\",\"SomeGuid\":\"{0}\"}}", guid);

            var context = new BindingContext
            {
                DestinationType = typeof(TestData),
                ValidModelBindingMembers = typeof(TestData).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(propertyInfo => propertyInfo.Name != "SomeString").Select(p => new BindingMemberInfo(p))
            };

            // When
            object actual;
            using (var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(source)))
            {
                IBodyDeserializer jilBodyDeserializer = new JilBodyDeserializer();
                actual = jilBodyDeserializer.Deserialize("application/json", bodyStream, context);
            }

            // Then
            Assert.IsInstanceOfType(actual, typeof(TestData));

            var actualData = actual as TestData;
            Assert.IsNotNull(actualData);
            Assert.IsNull(actualData.SomeString);
            Assert.AreEqual(guid, actualData.SomeGuid);
        }

        [TestMethod]
        public void when_deserializing_with_list()
        {
            // Given
            var guid = Guid.NewGuid();

            // \"MyStringArray\":[\"somestring1\",\"somestring2\"]
            string source = string.Format("{{\"SomeString\":\"some string value\",\"SomeGuid\":\"{0}\",\"SomeList\":[\"somestring1\",\"somestring2\"]}}", guid);

            var context = new BindingContext
            {
                DestinationType = typeof(TestDataWithList),
                ValidModelBindingMembers = typeof(TestDataWithList).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new BindingMemberInfo(p)),
            };

            // When
            object actual;
            using (var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(source)))
            {
                IBodyDeserializer jilBodyDeserializer = new JilBodyDeserializer();
                actual = jilBodyDeserializer.Deserialize("application/json", bodyStream, context);
            }

            // Then
            Assert.IsInstanceOfType(actual, typeof(TestDataWithList));

            var actualData = actual as TestDataWithList;
            Assert.IsNotNull(actualData);
            Assert.AreEqual("some string value", actualData.SomeString);
            Assert.AreEqual(guid, actualData.SomeGuid);
            Assert.IsNotNull(actualData.SomeList);
            Assert.AreEqual(2, actualData.SomeList.Count);
            Assert.AreEqual("somestring1", actualData.SomeList[0]);
            Assert.AreEqual("somestring2", actualData.SomeList[1]);
        }
    }
}
