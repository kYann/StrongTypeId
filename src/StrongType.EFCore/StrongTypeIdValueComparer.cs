using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace StrongType.EFCore;

public class StrongTypeIdValueComparer<TStrongType, TValue> : ValueComparer<TStrongType> where TStrongType : StrongTypeId<TValue>
{
    static readonly Func<TValue, object> factory = StrongTypeIdHelper.GetFactory<TValue>(typeof(TStrongType));

    public StrongTypeIdValueComparer() :
        base(
        (st1, st2) => (st1 == null ? 
            st2 == null ? true : st2.Equals(st1) 
        : st1.Equals(st2)),
        (st) => st.GetHashCode(),
        st => (TStrongType)factory(st.Value)
        )
    { }
}
