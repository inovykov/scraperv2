using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Interfaces.Wrappers;

namespace Wrappers
{
    public class JsonConverterWrapper : IJsonConverterWrapper
    {
        public T DeserializeObject<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj);
        }

        public T DeserializeObjectSafe<T>(string obj)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                Error = delegate(object sender, ErrorEventArgs args)
                {
                    args.ErrorContext.Handled = true;
                }
            };

            return JsonConvert.DeserializeObject<T>(obj, serializerSettings);
        }

        public string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
