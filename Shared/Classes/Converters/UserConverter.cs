using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.Classes.Shared.Classes;


namespace Shared.Classes.Converters
{
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            try
            {
                var jObject = JObject.Load(reader);
                var target = Create(objectType, jObject);
                serializer.Populate(jObject.CreateReader(), target);
                return target;
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    public class UserConverter : JsonCreationConverter<User>
    {
        protected override User Create(Type objectType, JObject jObject)
        {
            try
            {
                Constants.UserTypes userType = (Constants.UserTypes)jObject["Type"].Value<int>();
                string login = jObject["Login"].Value<string>();
                string password = jObject["Password"].Value<string>();
                switch (userType)
                {
                    case Constants.UserTypes.RegularUser:
                        return new RegularUser(login, password);
                    case Constants.UserTypes.Admin:
                        return new AdminUser(login, password);
                    default: throw new NotSupportedException("Unsupported user type: {userType}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                return null;
            }
        }
    }
}
