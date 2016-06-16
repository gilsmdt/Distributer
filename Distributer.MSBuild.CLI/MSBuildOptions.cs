using Commons.GetOptions;

[assembly: Commons.UsageComplement("\r\nDistributes build projects to multiple machines")]
[assembly: Commons.AdditionalInfo("example Distributer.MSBuild.CLI.exe -d distributer.json -b build.json -v 5773")]

namespace Distributer.MSBuild.CLI
{
    public class MSBuildOptions : Options
    {
        public bool Valid { get; } = false;

        [Option("specify the path to the distributer config file (default is distributer.json)", ShortForm = 'd', AlternateForm = "distributer-config")]
        public string DistributerConfigPath { get; set; }

        [Option("specify the path to the build config file (default is build.json)", ShortForm = 'b', AlternateForm = "build-config")]
        public string BuildConfigPath { get; set; }

        [Option("specify the changeset version to get", ShortForm = 'v', AlternateForm = "version-number")]
        public string VersionNumber { get; set; }

        [Option("show usage", ShortForm = 'h', AlternateForm = "help")]
        public bool ShowUsage { get; set; }


        public MSBuildOptions(string[] args) : base(new OptionsContext(), args)
        {
            if (ShowUsage)
            {
                Valid = false;
                base.DoHelp();
            }
            else
            {
                Valid = true;
            }
        }
    }
}
