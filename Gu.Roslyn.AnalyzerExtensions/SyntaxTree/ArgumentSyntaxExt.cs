namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension methods for <see cref="ArgumentSyntax"/>.
    /// </summary>
    public static class ArgumentSyntaxExt
    {
        /// <summary>
        /// Try get the value of the argument if it is a constant string.
        /// </summary>
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The string contents of <paramref name="argument"/>.</param>
        /// <returns>True if the argument expression was a constant string.</returns>
        public static bool TryGetStringValue(this ArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken, out string result)
        {
            result = null;
            return argument?.Expression is ExpressionSyntax expression &&
                   expression.TryGetStringValue(semanticModel, cancellationToken, out result);
        }

        /// <summary>
        /// Try get the value of the argument if it is a typeof() call.
        /// </summary>
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The string contents of <paramref name="argument"/>.</param>
        /// <returns>True if the call is typeof() and we could figure out the type.</returns>
        public static bool TryGetTypeofValue(this ArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken, out ITypeSymbol result)
        {
            result = null;
            return argument?.Expression is TypeOfExpressionSyntax expression &&
                   semanticModel.TryGetType(expression.Type, cancellationToken, out result);
        }

        /// <summary>
        /// Find the matching parameter for the argument.
        /// </summary>
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <param name="method">The <see cref="BaseMethodDeclarationSyntax"/>.</param>
        /// <param name="parameter">The matching <see cref="ParameterSyntax"/>.</param>
        /// <returns>True if a matching parameter was found.</returns>
        public static bool TryFindParameter(this ArgumentSyntax argument, BaseMethodDeclarationSyntax method, out ParameterSyntax parameter)
        {
            return method.TryFindParameter(argument, out parameter);
        }

        /// <summary>
        /// Find the matching parameter for the argument.
        /// </summary>
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <param name="method">The <see cref="IMethodSymbol"/>.</param>
        /// <param name="parameter">The matching <see cref="ParameterSyntax"/>.</param>
        /// <returns>True if a matching parameter was found.</returns>
        public static bool TryFindParameter(this ArgumentSyntax argument, IMethodSymbol method, out IParameterSymbol parameter)
        {
            return method.TryFindParameter(argument, out parameter);
        }
    }
}
