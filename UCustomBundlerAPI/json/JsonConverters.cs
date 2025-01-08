using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
namespace ScottyFoxArt.UnityBundler.Json
{
    public class UnityBundle_Reference_JsonConverter : JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            Debug.Log($"Deserializing -> {objectType.Name}");
            if (reader.TokenType == JsonToken.Null)
            {
                Debug.Log($"Null Token , Defaulting Value.");
                return existingValue;
            }
            var path = reader.Value.ToString();
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.Log($"Empty Value , Defaulting Value.");
                return existingValue;
            }
            BundleRegistry.TryGet_Reference(path, out var result, false, objectType.Name);
            if (result != null && objectType.IsAssignableFrom(result.GetType()))
                return result;
            Debug.Log($"Could Not Find Path");
            try
            {
                var obj = JObject.Load(reader).ToObject(objectType, serializer);
                if (obj != null)
                {
                    Debug.Log($"Deserialized Non-Path Value");
                    return obj;
                }
            }
            catch
            {
                Debug.Log($"Failed Default Serialization.");
            }
            Debug.Log($"Invalid Json , Defaulting Value.");
            return existingValue;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class UnityColorJsonConverter : JsonConverter<Color>
    {
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default;
            Debug.Log(reader.TokenType.ToString());
            var htmlRBG = reader.Value?.ToString();
            Debug.Log(htmlRBG);
            Color color = existingValue;
            ColorUtility.TryParseHtmlString(htmlRBG, out color);
            return color;
        }
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            var color = value;
            if (color.a == 1)
                writer.WriteValue(ColorUtility.ToHtmlStringRGBA(color));
            else
                writer.WriteValue(ColorUtility.ToHtmlStringRGB(color));
        }
    }
}