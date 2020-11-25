using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using StrongType.NHibernate;
using System;

namespace StrongType.FluentNHibernate
{
	public class ConventionStrongTypeId : IUserTypeConvention, IIdConvention
	{
		private bool IsStrongTypeId(IPropertyInspector propertyInspector)
		{
			var systemType = propertyInspector.Type.GetUnderlyingSystemType();
			if (systemType is null)
				return false;

			return StrongTypeIdHelper.IsStrongTypeId(systemType);
		}

		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
		{
			criteria.Expect(IsStrongTypeId);
		}

		public void Apply(IPropertyInstance instance)
		{
			var propertyType = instance.Type.GetUnderlyingSystemType();

			var customType = typeof(NHStrongTypeIdUserType<>).MakeGenericType(propertyType);

			instance.CustomType(customType);
		}

		public void Apply(IIdentityInstance instance)
		{
			var systemType = instance.Type.GetUnderlyingSystemType();
			if (systemType is null)
				return;

			var isStrongType = StrongTypeIdHelper.IsStrongTypeId(systemType);

			if (isStrongType)
			{
				var customType = typeof(NHStrongTypeIdUserType<>).MakeGenericType(systemType);
				instance.CustomType(customType);
			}
		}
	}
}
