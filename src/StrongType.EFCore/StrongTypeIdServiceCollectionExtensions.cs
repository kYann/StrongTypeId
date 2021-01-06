using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrongType.EFCore
{
	public static class StrongTypeIdServiceCollectionExtensions
	{
		public static IServiceCollection AddEntityFrameworkStrongTypeIdConventions(this IServiceCollection serviceCollection)
		{
			new EntityFrameworkServicesBuilder(serviceCollection)
				.TryAdd<IConventionSetPlugin, StrongTypeConventionSetPlugin>();

			return serviceCollection;
		}
	}
}
