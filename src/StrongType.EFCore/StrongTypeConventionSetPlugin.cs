using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrongType.EFCore
{
	public class StrongTypeConventionSetPlugin : IConventionSetPlugin
	{
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			var convention = new StrongTypeIdConversionConvention();
			conventionSet.EntityTypeAddedConventions.Insert(0, convention);
			conventionSet.PropertyAddedConventions.Add(convention);

			return conventionSet;
		}
	}
}
