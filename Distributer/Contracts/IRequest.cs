using Distributer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributer.Contracts
{
    public interface IRequest
    {
        Machine Machine { get; set; }
        int Id { get; set; }
    }
}
