using System.IO;
using Newtonsoft.Json;
using Shared.Classes.Converters;
using Shared.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Classes.Repositories.FileType
{
    public class MessageFileRepository : IMessageRepository
    {
        private readonly string _messagesPath;
        public MessageFileRepository(string messagesPath)
        {
            _messagesPath = messagesPath;
        }
        public Dictionary<string, List<Message>> GetAllMesasges()
        {
            if (!File.Exists(_messagesPath))
            {
                return new Dictionary<string, List<Message>>();
            }
            using (var reader = new StreamReader(_messagesPath))
            {
                var json = reader.ReadToEnd();

                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new UserConverter() }
                };

                return JsonConvert.DeserializeObject<Dictionary<string, List<Message>>>(json, settings) ?? new Dictionary<string, List<Message>>();
            }
        }
        public void SaveMessages(Dictionary<string, List<Message>> messages)
        {
            try
            {
                using (var writer = new StreamWriter(_messagesPath))
                {
                    var json = JsonConvert.SerializeObject(messages, Formatting.Indented);
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
