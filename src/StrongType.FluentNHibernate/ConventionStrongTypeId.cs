using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using StrongType.NHibernate;
using System;

namespace StrongType.FluentNHibernate
{
	public class ConventionStrongTypeId : IUserTypeConvention
	{
		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
		{
			criteria.Expect(_ => typeof(IStrongTypeId).IsAssignableFrom(_.Type.GetUnderlyingSystemType()));
		}

		public void Apply(IPropertyInstance instance)
		{
			var propertyType = instance.Type.GetUnderlyingSystemType();

			var customType = typeof(NHStrongTypeIdUserType<>).MakeGenericType(propertyType);

			instance.CustomType(customType);
		}
	}
}
