using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolutionObjectModel
{
	[DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
	public sealed class Project
	{
		public string ProjectName { get; private set; }
		public string RelativePath { get; private set; }
		public Guid ProjectGuid { get; private set; }
		public ProjectType ProjectType { get; private set; }
		public XDocument Document { get; private set; }
		public IDictionary<string, ProjectConfiguration> Configurations { get; private set; }

		private readonly Lazy<IList<Project>> _references;
		public IList<Project> References { get { return _references != null ? _references.Value : null; } }

		private readonly Solution _solution;

		internal Project(string solutionDir, Solution solution, object project)
		{
			_solution = solution;
			ProjectName = (string)project.GetProperty("ProjectName");
			RelativePath = (string)project.GetProperty("RelativePath");
			ProjectGuid = Guid.Parse((string)project.GetProperty("ProjectGuid"));
			ProjectType = (ProjectType)Enum.Parse(typeof(ProjectType), project.GetProperty("ProjectType").ToString());

			Configurations = new Dictionary<string, ProjectConfiguration>();
			var configurations = (IDictionary)project.GetProperty("ProjectConfigurations");
			foreach (DictionaryEntry configuration in configurations)
			{
				Configurations.Add((string)configuration.Key, new ProjectConfiguration(configuration.Value));
			}

			if (ProjectType == ProjectType.KnownToBeMSBuildFormat)
			{
				var resolver = new XmlNamespaceManager(new NameTable());
				resolver.AddNamespace("ms", "http://schemas.microsoft.com/developer/msbuild/2003");

				Document = XDocument.Load(Path.Combine(solutionDir, RelativePath));
				foreach (var propertyGroupNode in Document.XPathSelectElements("/ms:Project/ms:PropertyGroup[@Condition][ms:OutputPath|ms:OutDir]", resolver))
				{
					var condition = propertyGroupNode.Attribute("Condition").Value;

					var match = Regex.Match(condition, @"^\s*'\$\(Configuration\)\|\$\(Platform\)'\s*==\s*'(?<configuration>[^|]+)\|(?<platform>[^']+)'\s*$", RegexOptions.ExplicitCapture);
					if (!match.Success)
					{
						continue;
					}

					var configurationName = match.Groups["configuration"].Value;
					var platformName = match.Groups["platform"].Value;

					var outputPath = propertyGroupNode.XPathSelectElement("ms:OutputPath|ms:OutDir", resolver).Value;
					if (outputPath.Last() != Path.DirectorySeparatorChar)
					{
						outputPath += Path.DirectorySeparatorChar;
					}

					var matchingConfigurations = Configurations.Values.Where(c => c.ConfigurationName == configurationName && c.PlatformName == platformName);
					foreach (var configuration in matchingConfigurations)
					{
						configuration.OutputPath = outputPath;
					}
				}

				_references = new Lazy<IList<Project>>(() => Document.XPathSelectElements("/ms:Project/ms:ItemGroup/ms:ProjectReference/ms:Project", resolver)
					.Select(r => _solution.Projects[Guid.Parse(r.Value)])
					.ToList());
			}
		}

		public override string ToString()
		{
			return ProjectName;
		}
	}
}
