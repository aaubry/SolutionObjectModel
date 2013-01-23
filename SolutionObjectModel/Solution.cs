using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SolutionObjectModel
{
	/// <summary>
	/// Solution class - uses reflection to access internal Microsoft class 
	/// that offers Solution parsing features.
	/// 
	/// Source - John Leidegren's answer to http://stackoverflow.com/questions/707107/library-for-parsing-visual-studio-solution-files
	/// </summary>
	/// <remarks>
	/// 
	/// Accesses Microsoft unpublished API's.  
	/// 
	/// DO NOT USE IN PRODUCTION APPLICATIONS
	/// 
	/// Intended for use in non-critical internal development tools only
	/// </remarks>
	public sealed class Solution
	{
		//internal class SolutionParser
		//Name: Microsoft.Build.Construction.SolutionParser
		//Assembly: Microsoft.Build, Version=4.0.0.0

		private static readonly Type _solutionParserType = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

		public IDictionary<Guid, Project> Projects { get; private set; }

		public Solution(string solutionFileName)
		{
			if (_solutionParserType == null)
			{
				throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
			}
			var solutionParser = _solutionParserType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);
			using (var streamReader = new StreamReader(solutionFileName))
			{
				solutionParser.SetProperty("SolutionReader", streamReader);
				solutionParser.InvokeMethod("ParseSolution");
			}

			Projects = new Dictionary<Guid, Project>();
			var solutionDir = Path.GetDirectoryName(solutionFileName);
			var array = (Array)solutionParser.GetProperty("Projects");
			for (int i = 0; i < array.Length; ++i)
			{
				var project = new Project(solutionDir, this, array.GetValue(i));
				Projects.Add(project.ProjectGuid, project);
			}
		}
	}
}
