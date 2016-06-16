using Distributer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributer.MSBuild.Data
{
    public class Project
    {
        public string TfsPath { get; set; }
        public string BuildProjectPath { get; set; }
        public string SourcePath { get; set; }
        public string BasePath { get; set; }
        public bool Override { get; set; }
    }
}
