using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace StrongType.HotChocolate
{
	public static class StrongTypeIdHotChocolateRegistration
	{
		public static IRequestExecutorBuilder BindStrongTypeId<TType>(this IRequestExecutorBuilder builder) where TType : IStrongTypeId
		{
			builder.BindRuntimeType<TType, StrongTypeIdGraphType<TType>>();

			return builder;
		}

		public static IRequestExecutorBuilder BindStrongTypeInAssembly(this IRequestExecutorBuilder builder, Assembly assembly)
		{
			foreach (var type in assembly.GetTypes())
			{
				if (StrongTypeIdHelper.IsStrongTypeId(type))
				{
					var graphType = StrongTypeIdGraphType.MakeType(type);
					builder.BindRuntimeType(type, graphType);
				}
			}

			return builder;
		}

		public static IRequestExecutorBuilder BindStrongTypeInAssemblies(this IRequestExecutorBuilder builder, IEnumerable<Assembly> assemblies)
		{
			foreach (var assembly in assemblies)
			{
				builder.BindStrongTypeInAssembly(assembly);
			}

			return builder;
		}
	}
}
