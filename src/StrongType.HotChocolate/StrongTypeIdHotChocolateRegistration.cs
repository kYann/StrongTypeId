using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
	}
}
