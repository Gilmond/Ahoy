using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.Swagger.Model;

namespace Swashbuckle.SwaggerGen.Generator
{
	public class SwaggerGeneratorOptions
	{
		public SwaggerGeneratorOptions()
		{
			ApiVersions = new List<Info> { new Info { Version = "v1", Title = "API V1" } };
			GroupNameSelector = (apiDesc) => apiDesc.GroupName;
			GroupNameComparer = Comparer<string>.Default;
			SecurityDefinitions = new Dictionary<string, SecurityScheme>();
			OperationFilters = new List<IOperationFilter>();
			DocumentFilters = new List<IDocumentFilter>();
			ResolveConflict = ThrowExceptionOnApiDescriptionConflict;
		}

		public IList<Info> ApiVersions { get; private set; }

		public Func<ApiDescription, string, bool> VersionSupportResolver { get; private set; }

		public bool IgnoreObsoleteActions { get; set; }

		public Func<ApiDescription, string> GroupNameSelector { get; set; }

		public IComparer<string> GroupNameComparer { get; set; }

		public IDictionary<string, SecurityScheme> SecurityDefinitions { get; private set; }

		public IList<IOperationFilter> OperationFilters { get; private set; }

		public IList<IDocumentFilter> DocumentFilters { get; private set; }

		internal Func<IEnumerable<ApiDescription>, string, ApiDescription> ResolveConflict { get; private set; }

		private static ApiDescription ThrowExceptionOnApiDescriptionConflict(IEnumerable<ApiDescription> apiDescriptions, string httpMethod)
		{
			throw new NotSupportedException(string.Format(
				"Multiple operations with path '{0}' and method '{1}'. Are you overloading action methods?",
				apiDescriptions.First().RelativePathSansQueryString(), httpMethod));
		}

		public void ResolveConflictsBy(Func<IEnumerable<ApiDescription>, string, ApiDescription> resolver)
			=> ResolveConflict = resolver ?? ThrowExceptionOnApiDescriptionConflict;

		public void SingleApiVersion(Info info)
		{
			ApiVersions.Clear();
			ApiVersions.Add(info);
			VersionSupportResolver = null;
		}

		public void MultipleApiVersions(
			IEnumerable<Info> apiVersions,
			Func<ApiDescription, string, bool> versionSupportResolver)
		{
			ApiVersions.Clear();
			foreach (var version in apiVersions)
			{
				ApiVersions.Add(version);
			}
			VersionSupportResolver = versionSupportResolver;
		}

		internal SwaggerGeneratorOptions Clone()
		{
			return new SwaggerGeneratorOptions
			{
				ApiVersions = ApiVersions,
				VersionSupportResolver = VersionSupportResolver,
				IgnoreObsoleteActions = IgnoreObsoleteActions,
				GroupNameSelector = GroupNameSelector,
				GroupNameComparer = GroupNameComparer,
				SecurityDefinitions = SecurityDefinitions,
				OperationFilters = OperationFilters,
				DocumentFilters = DocumentFilters,
				ResolveConflict = ResolveConflict
			};
		}
	}
}