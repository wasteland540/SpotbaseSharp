using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SpotbaseSharp.ViewModels
{
    public static class ViewModelBaseExtension
    {
        public static string GetPropertyNameFromExpression<T>(
            this object target,
            Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException();
            }

            MemberExpression memberExpression = FindMemberExpression(expression);

            if (memberExpression == null)
            {
                throw new ArgumentException("Wrong Expression");
            }

            var member = memberExpression.Member as PropertyInfo;
            if (member == null)
            {
                throw new ArgumentException("Wrong Expression");
            }

            if (member.DeclaringType == null)
            {
                throw new ArgumentException("Wrong Expression");
            }

            if (target != null && !member.DeclaringType.IsInstanceOfType(target))
            {
                throw new ArgumentException("Wrong Expression");
            }

            if (member.GetGetMethod(true).IsStatic)
            {
                throw new ArgumentException("Wrong Expression");
            }

            return member.Name;
        }

        private static MemberExpression FindMemberExpression<T>(Expression<Func<T>> expression)
        {
            var body = expression.Body as UnaryExpression;
            if (body != null)
            {
                UnaryExpression unary = body;
                var member = unary.Operand as MemberExpression;
                if (member == null)
                    throw new ArgumentException("Wrong Unary Expression");
                return member;
            }

            return expression.Body as MemberExpression;
        }
    }
}