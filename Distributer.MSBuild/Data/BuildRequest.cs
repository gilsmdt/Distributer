using Distributer.Contracts;
using Distributer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributer.MSBuild.Data
{
    public class BuildRequest : IRequest
    {
        public Machine Machine { get; set; }
        public int Id { get; set; }
        public Project Project { get; set; }        
    }
}
