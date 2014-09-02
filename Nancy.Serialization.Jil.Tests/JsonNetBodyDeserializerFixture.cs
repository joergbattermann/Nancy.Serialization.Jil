using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nancy.ModelBinding;

namespace Nancy.Serialization.Jil.Tests
{
    [TestClass]
    public class JsonNetBodyDeserializerFixture
    {
        public class TestData
        {
            private TestData()
            {
            }

            public TestData(string randomStuff)
            {
                // Should never get here as it should use the NonPublicDefaultConstructor first.
                if (randomStuff == null)
                    throw new ArgumentNullException("randomStuff");
            }

            public string SomeString { get; set; }

            public Guid SomeGuid { get; set; }
        }

        [TestMethod]
        public void when_deserializing()
        {
            // Given
            var guid = Guid.NewGuid();
            string source = string.Format("{{\"someString\":\"some string value\",\"someGuid\":\"{0}\"}}", guid);

            var context = new BindingContext
            {
                DestinationType = typeof(TestData),
                ValidModelProperties = typeof(TestData).GetProperties(BindingFlags.Public | BindingFlags.Instance),
            };

            // When
            object actual;
            using (var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(source)))
            {
                IBodyDeserializer sut = new JilBodyDeserializer();
                actual = sut.Deserialize("application/json", bodyStream, context);
            }

            // Then
            Assert.IsInstanceOfType(actual, typeof(TestData));

            var actualData = actual as TestData;
            Assert.IsNotNull(actualData);
            Assert.AreEqual("some string value", actualData.SomeString);
            Assert.AreEqual(guid, actualData.SomeGuid);
        }
    }
}
