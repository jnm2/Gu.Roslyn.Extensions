namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="ExpressionSyntax"/>.
    /// </summary>
    public static class ExpressionSyntaxExt
    {
        /// <summary>
        /// Try get the value of the argument if it is a constant string.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="text">The string contents of <paramref name="expression"/>.</param>
        /// <returns>True if the argument expression was a constant string.</returns>
        public static bool TryGetStringValue(this ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken, out string text)
        {
            text = null;
            if (expression == null)
            {
                return false;
            }

            switch (expression)
            {
                case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
                    text = literal.Token.ValueText;
                    return true;
                case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.NullLiteralExpression):
                    text = null;
                    return true;
                case CastExpressionSyntax cast:
                    return TryGetStringValue(cast.Expression, semanticModel, cancellationToken, out text);
                case InvocationExpressionSyntax invocation when invocation.IsNameOf():
                    if (invocation.ArgumentList != null &&
                        invocation.ArgumentList.Arguments.TrySingle(out var nameofArg))
                    {
                        switch (nameofArg.Expression)
                        {
                            case IdentifierNameSyntax identifierName:
                                text = identifierName.Identifier.ValueText;
                                return true;
                            case MemberAccessExpressionSyntax memberAccess:
                                text = memberAccess.Name.Identifier.ValueText;
                                return true;
                            default:
                                var constantValue = semanticModel.GetConstantValueSafe(invocation, cancellationToken);
                                if (constantValue.HasValue &&
                                    constantValue.Value is string s)
                                {
                                    text = s;
                                    return true;
                                }

                                break;
                        }
                    }

                    return false;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Name.Identifier.ValueText == "Empty":
                    switch (memberAccess.Expression)
                    {
                        case PredefinedTypeSyntax predefinedType when predefinedType.Keyword.ValueText == "string":
                            text = string.Empty;
                            return true;
                        case IdentifierNameSyntax source when string.Equals(source.Identifier.ValueText, "string", StringComparison.OrdinalIgnoreCase):
                            text = string.Empty;
                            return true;
                    }

                    return false;
            }

            return semanticModel.TryGetConstantValue(expression, cancellationToken, out text);
        }

        /// <summary>
        /// Check if <paramref name="expression"/> is <paramref name="destination"/>.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="destination">The other <see cref="ITypeSymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if <paramref name="expression"/> is <paramref name="destination"/>. </returns>
        public static bool IsAssignableTo(this ExpressionSyntax expression, ITypeSymbol destination, SemanticModel semanticModel)
        {
            if (expression == null || destination == null)
            {
                return false;
            }

            return semanticModel.ClassifyConversion(expression, destination).IsImplicit;
        }

        /// <summary>
        /// Check if <paramref name="expression"/> is <paramref name="destination"/>.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="destination">The other <see cref="QualifiedType"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if <paramref name="expression"/> is <paramref name="destination"/>. </returns>
        public static bool IsAssignableTo(this ExpressionSyntax expression, QualifiedType destination, SemanticModel semanticModel)
        {
            if (expression == null ||
                destination == null ||
                semanticModel?.Compilation == null)
            {
                return false;
            }

            return IsAssignableTo(expression, destination.GetTypeSymbol(semanticModel.Compilation), semanticModel);
        }

        /// <summary>
        /// Check if <paramref name="expression"/> is <paramref name="destination"/>.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="destination">The other <see cref="ITypeSymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if <paramref name="expression"/> is <paramref name="destination"/>. </returns>
        public static bool IsSameType(this ExpressionSyntax expression, ITypeSymbol destination, SemanticModel semanticModel)
        {
            if (expression == null || destination == null)
            {
                return false;
            }

            return semanticModel.ClassifyConversion(expression, destination).IsIdentity;
        }

        /// <summary>
        /// Check if <paramref name="expression"/> is <paramref name="destination"/>.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="destination">The other <see cref="QualifiedType"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if <paramref name="expression"/> is <paramref name="destination"/>. </returns>
        public static bool IsSameType(this ExpressionSyntax expression, QualifiedType destination, SemanticModel semanticModel)
        {
            if (expression == null ||
                destination == null ||
                semanticModel?.Compilation == null)
            {
                return false;
            }

            return IsSameType(expression, destination.GetTypeSymbol(semanticModel.Compilation), semanticModel);
        }

        /// <summary>
        /// Check if (destination)(object)expression will work.
        /// </summary>
        /// <param name="expression">The expression containing the value.</param>
        /// <param name="destination">The type to cast to.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if a boxed instance can be cast.</returns>
        public static bool IsRepresentationPreservingConversion(this ExpressionSyntax expression, ITypeSymbol destination, SemanticModel semanticModel)
        {
            return semanticModel.IsRepresentationPreservingConversion(expression, destination);
        }
    }
}
