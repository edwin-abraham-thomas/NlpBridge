using System.Linq.Expressions;

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

        public static void SetValue<TObject, TProperty>(TObject obj, Expression<Func<TObject, TProperty>> expression, TProperty value)
        {
            ParameterExpression valueParameterExpression = Expression.Parameter(typeof(TProperty));
            Expression targetExpression = expression.Body is UnaryExpression ? ((UnaryExpression)expression.Body).Operand : expression.Body;

            var assign = Expression.Lambda<Action<TObject, TProperty>>
                        (
                            Expression.Assign(targetExpression, Expression.Convert(valueParameterExpression, targetExpression.Type)),
                            expression.Parameters.Single(),
                            valueParameterExpression
                        );

            assign.Compile().Invoke(obj, value);
        }
    }
}
