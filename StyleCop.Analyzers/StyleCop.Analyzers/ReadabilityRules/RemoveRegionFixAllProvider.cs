﻿namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Helpers;

    internal sealed class RemoveRegionFixAllProvider : DocumentBasedFixAllProvider
    {
        protected override string CodeActionTitle => "Remove region";

        protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
        {
            try
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                SyntaxNode root = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                var nodesToRemove = diagnostics.Select(d => root.FindNode(d.Location.SourceSpan, findInsideTrivia: true))
                    .Where(node => node != null && !node.IsMissing)
                    .OfType<RegionDirectiveTriviaSyntax>()
                    .SelectMany(node => node.GetRelatedDirectives())
                    .Where(node => !node.IsMissing);

                return root.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.AddElasticMarker);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
