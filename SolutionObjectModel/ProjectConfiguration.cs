using System.Diagnostics;

namespace SolutionObjectModel
{
	[DebuggerDisplay("{ConfigurationName}, {PlatformName}")]
	public sealed class ProjectConfiguration
	{
		public bool IncludeInBuild { get; private set; }
		public string ConfigurationName { get; private set; }
		public string FullName { get; private set; }
		public string PlatformName { get; private set; }
		public string OutputPath { get; internal set; }

		internal ProjectConfiguration(object projectConfiguration)
		{
			IncludeInBuild = (bool)projectConfiguration.GetProperty("IncludeInBuild");
			ConfigurationName = (string)projectConfiguration.GetProperty("ConfigurationName");
			FullName = (string)projectConfiguration.GetProperty("FullName");
			PlatformName = (string)projectConfiguration.GetProperty("PlatformName");
		}

		public override string ToString()
		{
			return FullName;
		}
	}
}