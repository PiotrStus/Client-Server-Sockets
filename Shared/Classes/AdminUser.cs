using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    namespace Shared.Classes
    {
        public class AdminUser : User
        {
            [JsonIgnore]
            public string FilesDirectory { get; private set; }
            public override Constants.UserTypes Type
            {
                get { return Constants.UserTypes.Admin; }
            }

            public AdminUser(string username, string password) : base(username, password)
            {
            }
        }
    }

}
