using Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Repository
{
    public interface IMessageRepository
    {
        Dictionary<string, List<Message>> GetAllMesasges();
        void SaveMessages(Dictionary<string, List<Message>> messages);
    }
}
