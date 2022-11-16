using KendoCoreService.Models.Request;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace KendoCoreService.Extensions
{
    public static class ExpressionExtension
    {
        /* Filter Expression */
        public static Expression<Func<T, bool>> GenerateFilterExpression<T>(Filter filter)
        {
            bool isFirst = true;
            Expression<Func<T, bool>> result = null;

            if (filter.Logic == "and")
            {
                foreach (var filterDesc in filter.Filters)
                {
                    if (filterDesc.IsDescriptor)
                    {
                        var operatorExpression = CreateOperatorExpression<T>(filterDesc.Operator, filterDesc.Field, filterDesc.Value);

                        if (!isFirst)
                        {
                            result = And(operatorExpression, result);
                        }
                        else
                        {
                            result = operatorExpression;
                            isFirst = false;
                        }
                    }
                    else
                    {
                        if (!isFirst)
                        {
                            result = And(GenerateFilterExpression<T>(filterDesc), result);
                        }
                        else
                        {
                            result = GenerateFilterExpression<T>(filterDesc);
                            isFirst = false;
                        }
                    }


                }
            }
            else if (filter.Logic == "or")
            {
                foreach (var filterDesc in filter.Filters)
                {
                    if (filterDesc.IsDescriptor)
                    {
                        var operatorExpression = CreateOperatorExpression<T>(filterDesc.Operator, filterDesc.Field, filterDesc.Value);

                        if (!isFirst)
                        {
                            result = Or(operatorExpression, result);
                        }
                        else
                        {
                            result = operatorExpression;
                            isFirst = false;
                        }
                    }
                    else
                    {
                        if (!isFirst)
                        {
                            result = Or(GenerateFilterExpression<T>(filterDesc), result);
                        }
                        else
                        {
                            result = GenerateFilterExpression<T>(filterDesc);
                            isFirst = false;
                        }
                    }
                }
            }

            return result;
        }

        public static LambdaExpression BuildLambda(Type objType, PropertyInfo pi)
        {
            var paramExpr = GetParameterExpression(objType);
            var propAccess = BuildAccessExpression(paramExpr, pi);
            return Expression.Lambda(propAccess, paramExpr);
        }

        private static ParameterExpression GetParameterExpression(Type objType)
        {
            return Expression.Parameter(objType, "f");
        }

        private static MemberExpression BuildAccessExpression(this ParameterExpression paramExpr, PropertyInfo pi)
        {
            return Expression.PropertyOrField(paramExpr, pi.Name);
        }

        private static UnaryExpression ConvertExpressionTypeToDouble(this Expression exp)
        {
            return Expression.Convert(exp, typeof(double));
        }

        private static UnaryExpression ConvertExpressionTypeToBoolean(this Expression exp)
        {
            return Expression.Convert(exp, typeof(bool));
        }

        private static UnaryExpression ConvertExpressionTypeToDateTime(this Expression exp)
        {
            return Expression.Convert(exp, typeof(DateTime));
        }

        private static ConstantExpression CreateConstantStringExpression(object value)
        {
            return Expression.Constant(value.ToString());
        }

        private static ConstantExpression CreateConstantDoubleExpression(object value)
        {
            return Expression.Constant(double.Parse(value.ToString()));
        }

        private static ConstantExpression CreateConstantBooleanExpression(object value)
        {
            return Expression.Constant(bool.Parse(value.ToString()));
        }

        private static ConstantExpression CreateConstantDateTimeExpression(object value)
        {
            return Expression.Constant(DateTime.Parse(value.ToString()));
        }

        private static UnaryExpression NotExpression(Expression exp)
        {
            return Expression.Not(exp);
        }

        private static BinaryExpression IsNotNullExpresion(Expression accessExp)
        {
            return Expression.NotEqual(accessExp, Expression.Constant(null));
        }

        private static BinaryExpression IsNullExpression(Expression accessExp)
        {
            return Expression.Equal(accessExp, Expression.Constant(null));
        }

        private static BinaryExpression NullCheckExpression(this Expression exp, Expression accessExp)
        {
            return Expression.AndAlso(IsNotNullExpresion(accessExp), exp);
        }

        private static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var param = GetParameterExpression(typeof(T));
            var body = Expression.AndAlso(
                Expression.Invoke(expr1, param),
                Expression.Invoke(expr2, param)
                );
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        private static Expression<Func<T, bool>> Or<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var param = GetParameterExpression(typeof(T));
            var body = Expression.OrElse(
                Expression.Invoke(expr1, param),
                Expression.Invoke(expr2, param)
                );
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        // Filter Operators

        private static Expression<Func<T, bool>> CreateOperatorExpression<T>(string op, string field, object value)
        {
            var paramExp = GetParameterExpression(typeof(T));
            var parsedOperator = FilteringExtension.GetOperator(op);
            var method = typeof(ExpressionExtension).GetMethod(parsedOperator, BindingFlags.NonPublic | BindingFlags.Static);

            var propInfo = CommonExtension.GetPropertyInfo(typeof(T), field);
            var accessExp = paramExp.BuildAccessExpression(propInfo);
            method = method.MakeGenericMethod(typeof(T));

            return (Expression<Func<T, bool>>)method.Invoke(null, new object[] { accessExp, paramExp, value });
        }

        private static Expression<Func<T, bool>> Equals<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            BinaryExpression exp;
            ConstantExpression constant;

            var typeName = accessExp.Type.Name;

            // For numeric values, use a double expression when comparing for equality.
            if (value.IsNumber() && !typeName.Equals(typeof(string).Name))
            {
                constant = CreateConstantDoubleExpression(value);
                accessExp = accessExp.ConvertExpressionTypeToDouble();
            }
            // For boolean values, use a boolean expression.
            else if (typeName.Equals(typeof(bool).Name))
            {
                constant = CreateConstantBooleanExpression(value);
                accessExp = accessExp.ConvertExpressionTypeToBoolean();
            }
            // For datetime values, use a datetime expression
            else if (typeName.Equals(typeof(DateTime).Name))
            {
                constant = CreateConstantDateTimeExpression(value);
                accessExp = accessExp.ConvertExpressionTypeToDateTime();
            }
            else
            {
                constant = CreateConstantStringExpression(value);
            }

            exp = Expression.Equal(accessExp, constant);

            return Expression.Lambda<Func<T, bool>>(exp, paramExp);
        }

        private static Expression<Func<T, bool>> DoesNotEqual<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            var exp = NotExpression(Equals<T>(accessExp, paramExp, value).Body);

            return Expression.Lambda<Func<T, bool>>(exp, paramExp);
        }

        private static Expression<Func<T, bool>> Contains<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            ConstantExpression constant = CreateConstantStringExpression(value);
            Expression exp = Expression.Call(accessExp, method, constant);

            return Expression.Lambda<Func<T, bool>>(exp.NullCheckExpression(accessExp), paramExp);
        }

        private static Expression<Func<T, bool>> DoesNotContain<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            ConstantExpression constant = CreateConstantStringExpression(value);
            Expression exp = NotExpression(Expression.Call(accessExp, method, constant));

            return Expression.Lambda<Func<T, bool>>(exp.NullCheckExpression(accessExp), paramExp);
        }

        private static Expression<Func<T, bool>> StartsWith<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            var method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            ConstantExpression constant = CreateConstantStringExpression(value);
            Expression exp = Expression.Call(accessExp, method, constant);

            return Expression.Lambda<Func<T, bool>>(exp.NullCheckExpression(accessExp), paramExp);
        }

        private static Expression<Func<T, bool>> EndsWith<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
            ConstantExpression constant = CreateConstantStringExpression(value);
            Expression exp = Expression.Call(accessExp, method, constant);

            return Expression.Lambda<Func<T, bool>>(exp.NullCheckExpression(accessExp), paramExp);
        }

        private static Expression<Func<T, bool>> IsEmpty<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            ConstantExpression constant = CreateConstantStringExpression(string.Empty);
            BinaryExpression exp = Expression.Equal(accessExp, constant);

            return Expression.Lambda<Func<T, bool>>(exp, paramExp);
        }

        private static Expression<Func<T, bool>> IsNotEmpty<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            ConstantExpression constant = CreateConstantStringExpression(string.Empty);
            BinaryExpression exp = Expression.NotEqual(accessExp, constant);

            return Expression.Lambda<Func<T, bool>>(exp, paramExp);
        }

        private static Expression<Func<T, bool>> IsNull<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            var typeName = accessExp.Type.Name;

            if (typeName.Equals(typeof(DateTime).Name))
            {
                accessExp = accessExp.ConvertExpressionTypeToDateTime();
            }

            BinaryExpression exp = IsNullExpression(accessExp);
            return Expression.Lambda<Func<T, bool>>(exp, paramExp);
        }

        private static Expression<Func<T, bool>> IsNotNull<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            var typeName = accessExp.Type.Name;

            if (typeName.Equals(typeof(DateTime).Name))
            {
                accessExp = accessExp.ConvertExpressionTypeToDateTime();
            }

            BinaryExpression exp = IsNotNullExpresion(accessExp);

            return Expression.Lambda<Func<T, bool>>(exp, paramExp);
        }

        private static Expression<Func<T, bool>> GreaterThan<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            ConstantExpression constant;

            var typeName = accessExp.Type.Name;

            // For numeric values, use a double expression when comparing for equality.
            if (typeName.Equals(typeof(DateTime).Name))
            {
                constant = CreateConstantDateTimeExpression(value);
                accessExp = accessExp.ConvertExpressionTypeToDateTime();
            }
            // For boolean values, use a boolean expression.
            else
            {
                constant = CreateConstantDoubleExpression(value);
                accessExp = accessExp.ConvertExpressionTypeToDouble();
            }

            BinaryExpression exp = Expression.GreaterThan(accessExp, constant);

            return Expression.Lambda<Func<T, bool>>(exp, paramExp);
        }

        private static Expression<Func<T, bool>> GreaterThanOrEqual<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            ConstantExpression constant;

            var typeName = accessExp.Type.Name;

            // For numeric values, use a double expression when comparing for equality.
            if (typeName.Equals(typeof(DateTime).Name))
            {
                constant = CreateConstantDateTimeExpression(value);
                accessExp = accessExp.ConvertExpressionTypeToDateTime();
            }
            // For boolean values, use a boolean expression.
            else
            {
                constant = CreateConstantDoubleExpression(value);
                accessExp = accessExp.ConvertExpressionTypeToDouble();
            }

            BinaryExpression exp = Expression.GreaterThanOrEqual(accessExp, constant);

            return Expression.Lambda<Func<T, bool>>(exp, paramExp);
        }

        private static Expression<Func<T, bool>> LessThan<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            ConstantExpression constant;

            var typeName = accessExp.Type.Name;

            // For numeric values, use a double expression when comparing for equality.
            if (typeName.Equals(typeof(DateTime).Name))
            {
                constant = CreateConstantDateTimeExpression(value);
                accessExp = accessExp.ConvertExpressionTypeToDateTime();
            }
            // For boolean values, use a boolean expression.
            else
            {
                constant = CreateConstantDoubleExpression(value);
                accessExp = accessExp.ConvertExpressionTypeToDouble();
            }

            BinaryExpression exp = Expression.LessThan(accessExp, constant);

            return Expression.Lambda<Func<T, bool>>(exp, paramExp);
        }

        private static Expression<Func<T, bool>> LessThanOrEqual<T>(Expression accessExp, ParameterExpression paramExp, object value)
        {
            ConstantExpression constant;

            var typeName = accessExp.Type.Name;

            // For numeric values, use a double expression when comparing for equality.
            if (typeName.Equals(typeof(DateTime).Name))
            {
                constant = CreateConstantDateTimeExpression(value);
                accessExp = accessExp.ConvertExpressionTypeToDateTime();
            }
            // For boolean values, use a boolean expression.
            else
            {
                constant = CreateConstantDoubleExpression(value);
                accessExp = accessExp.ConvertExpressionTypeToDouble();
            }

            BinaryExpression exp = Expression.LessThanOrEqual(accessExp, constant);

            return Expression.Lambda<Func<T, bool>>(exp, paramExp);
        }
    }
}
