using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.data
{
    public interface IDatabaseConnectionFactory
    {
         public IDbConnection CreateConnection();
    }
}
