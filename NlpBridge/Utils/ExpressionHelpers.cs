using System.Linq.Expressions;
using System.Reflection;

namespace NlpBridge.Utils
{
    public static class ExpressionHelpers
    {
        public static TPropertyType GetValue<TObject, TPropertyType>(TObject obj, Expression<Func<TObject, TPropertyType>> propertyExpression)
        {
            var getterLambda = Expression.Lambda<Func<TObject, TPropertyType>>(propertyExpression.Body, propertyExpression.Parameters);
            var getter = getterLambda.Compile();
            var value = getter(obj);
            return getter(obj);
        }

        public static TObject SetValue<TObject, TPropertyType>(TObject obj, TPropertyType value, Expression<Func<TObject, TPropertyType>> propertyExpression)
        {
            //var memberExpression = (MemberExpression)propertyExpression.Body;
            //var propertyInfo = (System.Reflection.PropertyInfo)memberExpression.Member;

            //var instanceParam = Expression.Parameter(typeof(TObject), "instance");
            //var valueParam = Expression.Parameter(typeof(TPropertyType), "value");

            //var assignment = Expression.Assign(Expression.Property(instanceParam, propertyInfo), valueParam);

            //var setterLambda = Expression.Lambda<Action<TObject, TPropertyType>>(assignment, instanceParam, valueParam);
            //var setter = setterLambda.Compile();

            //setter(obj, value);

            var memberSelectorExpression = propertyExpression.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(obj, value, null);
                }
            }

            return obj;
        }
    }
}
