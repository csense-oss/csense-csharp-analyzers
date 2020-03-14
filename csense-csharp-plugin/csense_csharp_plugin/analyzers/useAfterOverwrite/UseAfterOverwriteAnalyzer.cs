using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using csenseCsharpAnalyzers.bll;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.Operations;
using csenseCsharpAnalyzers;

namespace csenseCsharpAnalyzers {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseAfterOverwriteAnalyzer : DiagnosticAnalyzer {

        public const string DiagnosticId = "UseAfterOverwriteAnalyzer";

        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            DiagnosticAnalyzerLocalizable.GetLocalizableStringFromResources(nameof(Resources.UseAfterOverwriteTitle)),
            DiagnosticAnalyzerLocalizable.GetLocalizableStringFromResources(nameof(Resources.UseAfterOverwriteFormat)),
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DiagnosticAnalyzerLocalizable.GetLocalizableStringFromResources(nameof(Resources.UseAfterOverwriteDescription)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context) {
            context.RegisterOperationAction(AnalyzeOperations, OperationKind.SimpleAssignment);
        }

        public void AnalyzeOperations(OperationAnalysisContext context) {
            var ops = (ISimpleAssignmentOperation)context.Operation;
            var target = ops.Target as ILocalReferenceOperation;
            var rhs = ops.Value as ILocalReferenceOperation;
            var containingBlock = ops.ParentOfType<IBlockOperation>();
            if (containingBlock == null || rhs == null || target == null) {
                return;
            }
            containingBlock.ForeachDescendantsOfType<ILocalReferenceOperation>((op) => {
                var isTarget = op.Local == target.Local;
                if (!isTarget) {
                    return;
                }
                if(op.Syntax.SpanStart <= ops.Syntax.SpanStart) {
                    return;
                }
                var containingStatement = op.ParentOfType<IExpressionStatementOperation>();
                if (containingStatement == null) {
                    return;
                }
                var usesRhs = containingStatement.AnyDescendantsOfType<ILocalReferenceOperation>(
                    (op) => op.Local == rhs.Local
                );
                if (usesRhs) {
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        containingStatement.Syntax.GetLocation(),
                        target.Local.Name, 
                        rhs.Local.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            });

        }
    }
}
