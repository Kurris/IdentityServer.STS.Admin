using System;
using System.Collections.Generic;

namespace IdentityServer.STS.Admin
{
    public class CommonEqual<TSource, TResult> : IEqualityComparer<TSource>
    {
        private readonly string _property;

        public CommonEqual(string property)
        {
            _property = property;
        }

        static CommonEqual()
        {
            _type = typeof(TSource);
        }

        static readonly Type _type;

        public bool Equals(TSource x, TSource y)
        {
            var xValue = (TResult) _type.GetProperty(_property).GetValue(x);
            var yValue = (TResult) _type.GetProperty(_property).GetValue(y);

            return xValue.Equals(yValue);
        }

        public int GetHashCode(TSource obj)
        {
            return base.GetHashCode();
        }
    }
}