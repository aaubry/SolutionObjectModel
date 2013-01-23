using SolutionObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			var solution = new Solution(@"D:\Work\SDB\ApplicationBlocks\Main\ApplicationBlocks.sln");
			foreach (var project in solution.Projects.Values)
			{
				Console.WriteLine("{0} - {1}", project.ProjectName, project.ProjectGuid);
				foreach (var configuration in project.Configurations)
				{
					Console.WriteLine("  {0}: {1} - {2}", configuration.Key, configuration.Value, configuration.Value.OutputPath);
				}
			}
		}
	}
}
