using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace CommonLib.Serialize
{
    public static class UniversalSerializer
    {
        public static T Deserialize<T>(string serializationType, string obj)
        {
            return serializationType.ToUpper() switch
            {
                SerializationType.Json => DeserializeJson<T>(obj),
                SerializationType.Xml => DeserializeXml<T>(obj),
                _ => throw new Exception("Wrong serialization type")
            };
        }

        public static string Serialize<T>(string serializationType, T obj)
        {
            return serializationType.ToUpper() switch
            {
                SerializationType.Json => SerializeJson(obj),
                SerializationType.Xml => SerializeXml(obj),
                _ => throw new Exception("Wrong serialization type")
            };
        }

        private static T DeserializeJson<T>(string obj) => JsonConvert.DeserializeObject<T>(obj);

        private static string SerializeJson<T>(T obj) => JsonConvert.SerializeObject(obj);

        private static T DeserializeXml<T>(string obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var sr = new StringReader(obj);
            using var xmlReader = XmlReader.Create(sr);

            return (T) serializer.Deserialize(xmlReader);
        }

        private static string SerializeXml<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            var namespaces = new XmlSerializerNamespaces(new[] {XmlQualifiedName.Empty});
            var settings = new XmlWriterSettings {OmitXmlDeclaration = true};
            using var sw = new StringWriter();
            using var xmlWriter = XmlWriter.Create(sw, settings);
            serializer.Serialize(xmlWriter, obj, namespaces);

            return sw.ToString();
        }
    }
}