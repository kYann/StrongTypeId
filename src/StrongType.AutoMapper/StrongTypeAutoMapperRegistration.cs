using AutoMapper;
using System.Reflection;

namespace StrongType.AutoMapper
{
    public static class StrongTypeAutoMapperRegistration
    {
		public static IMapperConfigurationExpression MapStrongTypeId<TType, TValueType>(this IMapperConfigurationExpression builder) 
			where TType : StrongTypeId<TValueType> 
			where TValueType : notnull
		{
			builder.CreateMap<TType, TValueType>().ConvertUsing<StrongTypeTypeToValueConverter<TType, TValueType>>();
			builder.CreateMap<TValueType, TType>().ConvertUsing<ValueToStrongTypeTypeConverter<TValueType, TType>>();

			return builder;
		}

		public static IMapperConfigurationExpression MapStrongTypeId(this IMapperConfigurationExpression cfg, Type strongTypeIdType, Type valueType)
		{
			if (StrongTypeIdHelper.IsStrongTypeId(strongTypeIdType) == false)
				throw new InvalidOperationException($"{strongTypeIdType} is not a strong type");

			var converterStrongToValueType = StrongTypeTypeToValueConverter.MakeType(strongTypeIdType, valueType);
			var converterValueToStrongType = ValueToStrongTypeTypeConverter.MakeType(valueType, strongTypeIdType);
			cfg.CreateMap(strongTypeIdType, valueType).ConvertUsing(converterStrongToValueType);
			cfg.CreateMap(valueType, strongTypeIdType).ConvertUsing(converterValueToStrongType);

			return cfg;
		}

		public static IMapperConfigurationExpression MapStrongTypeInAssembly(this IMapperConfigurationExpression cfg, Assembly assembly)
		{
			foreach (var type in assembly.GetTypes())
			{
				if (StrongTypeIdHelper.IsStrongTypeId(type))
				{
					var valueType = type.GenericTypeArguments[0];

					cfg = cfg.MapStrongTypeId(type, valueType);
				}
			}

			return cfg;
		}

		public static IMapperConfigurationExpression MapStrongTypeInAssemblies(this IMapperConfigurationExpression builder, IEnumerable<Assembly> assemblies)
		{
			foreach (var assembly in assemblies)
			{
				builder.MapStrongTypeInAssembly(assembly);
			}

			return builder;
		}
	}
}