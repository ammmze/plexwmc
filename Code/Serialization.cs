using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PlexWMC
{
    static class Serialization
    {

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            /*StringWriter writer = new StringWriter();
            (new System.Xml.Serialization.XmlSerializer(obj.GetType())).Serialize(writer, obj);
            return writer.ToString();-*/
        }


        public static T Deserialize<T>(string obj) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(obj);
            /*XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextReader reader = new StringReader(obj);
            return (T) serializer.Deserialize(reader);*/
        }

    }
}
