//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from GiraphParser.g4 by ANTLR 4.7.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="GiraphParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
//[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
//[System.CLSCompliant(false)]
public interface IGiraphParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.start"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStart([NotNull] GiraphParser.StartContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProgram([NotNull] GiraphParser.ProgramContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.dcls"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDcls([NotNull] GiraphParser.DclsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.objectDcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitObjectDcl([NotNull] GiraphParser.ObjectDclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.variableDcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariableDcl([NotNull] GiraphParser.VariableDclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.graphInitDcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGraphInitDcl([NotNull] GiraphParser.GraphInitDclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.graphDclBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGraphDclBlock([NotNull] GiraphParser.GraphDclBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.vertexDcls"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVertexDcls([NotNull] GiraphParser.VertexDclsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.vertexDcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVertexDcl([NotNull] GiraphParser.VertexDclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.edgeDcls"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEdgeDcls([NotNull] GiraphParser.EdgeDclsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.edgeDcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEdgeDcl([NotNull] GiraphParser.EdgeDclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.assignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignment([NotNull] GiraphParser.AssignmentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression([NotNull] GiraphParser.ExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.expressionExtension"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionExtension([NotNull] GiraphParser.ExpressionExtensionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.query"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQuery([NotNull] GiraphParser.QueryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.querySC"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQuerySC([NotNull] GiraphParser.QuerySCContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.noReturnQuery"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNoReturnQuery([NotNull] GiraphParser.NoReturnQueryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.returnQuery"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReturnQuery([NotNull] GiraphParser.ReturnQueryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperator([NotNull] GiraphParser.OperatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.simpleOperators"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSimpleOperators([NotNull] GiraphParser.SimpleOperatorsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.advancedOperators"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAdvancedOperators([NotNull] GiraphParser.AdvancedOperatorsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.formalParams"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFormalParams([NotNull] GiraphParser.FormalParamsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.formalParam"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFormalParam([NotNull] GiraphParser.FormalParamContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.functionDcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunctionDcl([NotNull] GiraphParser.FunctionDclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.codeBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCodeBlock([NotNull] GiraphParser.CodeBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.returnBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReturnBlock([NotNull] GiraphParser.ReturnBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.runFunction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRunFunction([NotNull] GiraphParser.RunFunctionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.codeBlockContent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCodeBlockContent([NotNull] GiraphParser.CodeBlockContentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.varOrConst"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVarOrConst([NotNull] GiraphParser.VarOrConstContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.varOrFuncOrConst"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVarOrFuncOrConst([NotNull] GiraphParser.VarOrFuncOrConstContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.variable"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariable([NotNull] GiraphParser.VariableContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.variableFunc"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariableFunc([NotNull] GiraphParser.VariableFuncContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.dotFunction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDotFunction([NotNull] GiraphParser.DotFunctionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.constant"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConstant([NotNull] GiraphParser.ConstantContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitString([NotNull] GiraphParser.StringContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.integer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInteger([NotNull] GiraphParser.IntegerContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.floatnum"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFloatnum([NotNull] GiraphParser.FloatnumContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.bool"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBool([NotNull] GiraphParser.BoolContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.objects"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitObjects([NotNull] GiraphParser.ObjectsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.ifElseIfElse"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIfElseIfElse([NotNull] GiraphParser.IfElseIfElseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.boolComparisons"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBoolComparisons([NotNull] GiraphParser.BoolComparisonsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.predicate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPredicate([NotNull] GiraphParser.PredicateContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.predicateCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPredicateCall([NotNull] GiraphParser.PredicateCallContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.where"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWhere([NotNull] GiraphParser.WhereContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.andOr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAndOr([NotNull] GiraphParser.AndOrContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.extend"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExtend([NotNull] GiraphParser.ExtendContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.select"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSelect([NotNull] GiraphParser.SelectContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.selectAll"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSelectAll([NotNull] GiraphParser.SelectAllContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.addQuery"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAddQuery([NotNull] GiraphParser.AddQueryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.addToGraph"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAddToGraph([NotNull] GiraphParser.AddToGraphContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.addToColl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAddToColl([NotNull] GiraphParser.AddToCollContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.loopDcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLoopDcl([NotNull] GiraphParser.LoopDclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.foreachLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForeachLoop([NotNull] GiraphParser.ForeachLoopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.whileLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWhileLoop([NotNull] GiraphParser.WhileLoopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.doWhileLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDoWhileLoop([NotNull] GiraphParser.DoWhileLoopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.forLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForLoop([NotNull] GiraphParser.ForLoopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.forCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForCondition([NotNull] GiraphParser.ForConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.assignmentParant"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignmentParant([NotNull] GiraphParser.AssignmentParantContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.forConditionInside"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForConditionInside([NotNull] GiraphParser.ForConditionInsideContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.operation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperation([NotNull] GiraphParser.OperationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.inlineDcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInlineDcl([NotNull] GiraphParser.InlineDclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.foreachCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForeachCondition([NotNull] GiraphParser.ForeachConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.allType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAllType([NotNull] GiraphParser.AllTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.allTypeWithColl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAllTypeWithColl([NotNull] GiraphParser.AllTypeWithCollContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.collectionDcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCollectionDcl([NotNull] GiraphParser.CollectionDclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.collectionAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCollectionAssignment([NotNull] GiraphParser.CollectionAssignmentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.setQuery"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSetQuery([NotNull] GiraphParser.SetQueryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.graphSetQuery"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGraphSetQuery([NotNull] GiraphParser.GraphSetQueryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.setExpressionAtri"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSetExpressionAtri([NotNull] GiraphParser.SetExpressionAtriContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.setExpressionVari"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSetExpressionVari([NotNull] GiraphParser.SetExpressionVariContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.compoundAssign"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCompoundAssign([NotNull] GiraphParser.CompoundAssignContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAttribute([NotNull] GiraphParser.AttributeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.setOneAttri"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSetOneAttri([NotNull] GiraphParser.SetOneAttriContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.collNoReturnOps"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCollNoReturnOps([NotNull] GiraphParser.CollNoReturnOpsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.collReturnOps"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCollReturnOps([NotNull] GiraphParser.CollReturnOpsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.enqueueOP"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnqueueOP([NotNull] GiraphParser.EnqueueOPContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.dequeueOP"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDequeueOP([NotNull] GiraphParser.DequeueOPContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.popOP"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPopOP([NotNull] GiraphParser.PopOPContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.pushOP"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPushOP([NotNull] GiraphParser.PushOPContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.extractMinOP"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExtractMinOP([NotNull] GiraphParser.ExtractMinOPContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.extractMaxOP"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExtractMaxOP([NotNull] GiraphParser.ExtractMaxOPContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.parameters"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameters([NotNull] GiraphParser.ParametersContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.printOptions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPrintOptions([NotNull] GiraphParser.PrintOptionsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.printOption"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPrintOption([NotNull] GiraphParser.PrintOptionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.print"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPrint([NotNull] GiraphParser.PrintContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.dequeueOPOneLine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDequeueOPOneLine([NotNull] GiraphParser.DequeueOPOneLineContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.commentLine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCommentLine([NotNull] GiraphParser.CommentLineContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GiraphParser.comments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComments([NotNull] GiraphParser.CommentsContext context);
}
