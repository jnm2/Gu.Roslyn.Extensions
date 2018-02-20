namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public class ParameterSymbolComparer : IEqualityComparer<IParameterSymbol>
    {
        public static readonly ParameterSymbolComparer Default = new ParameterSymbolComparer();

        private ParameterSymbolComparer()
        {
        }

        public static bool Equals(IParameterSymbol x, IParameterSymbol y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null ||
                y == null)
            {
                return false;
            }

            return x.MetadataName == y.MetadataName &&
                   SymbolComparer.Equals(x.ContainingSymbol, y.ContainingSymbol);
        }

        /// <inheritdoc />
        bool IEqualityComparer<IParameterSymbol>.Equals(IParameterSymbol x, IParameterSymbol y) => Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(IParameterSymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }

        // ReSharper disable once UnusedMember.Local
        [Obsolete("Should only be called with arguments of type IParameterSymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
    }
}
