using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Common
{
    public static class JsonUtils
    {
        public static string Serialize(object value)
        {
            string output = JsonConvert.SerializeObject(
                value,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            return output;
        }
    }
}
