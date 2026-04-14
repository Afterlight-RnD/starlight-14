// // SPDX-FileCopyrightText: 2026 Starlight Network
// // SPDX-License-Identifier: Starlight-MIT
//
// using System.Collections.Generic;
// using System.Collections.Immutable;
// using System.Text;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
// using Microsoft.CodeAnalysis.Text;
//
// namespace Content.Starlight.Codegen;
//
// //[Generator]
// public sealed class StructUnionGenerator : IIncrementalGenerator
// {
//     public const string AttributeName = "Content.Shared.Starlight.Abstract.Codegen.StructUnionAttribute`1";
//     public const string AttributeHint = "StructUnion.g.cs";
//
//     public void Initialize(IncrementalGeneratorInitializationContext context)
//     {
//         // Add the marker attribute to the compilation
//         context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
//             AttributeHint,
//             SourceText.From(GenerateStaticData(), Encoding.UTF8)));
//
//         var classDeclarations = context.SyntaxProvider
//             .ForAttributeWithMetadataName(
//                 AttributeName,
//                 (node, _) => node is StructDeclarationSyntax,
//                 (ctx, _) => (Struct: (INamedTypeSymbol)ctx.TargetSymbol, ctx.Attributes))
//             .Collect();
//
//         var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations);
//
//         context.RegisterSourceOutput(compilationAndClasses,
//             (spc, source) => Execute(spc, source.Left, source.Right));
//     }
//
//     private void Execute(SourceProductionContext context, Compilation compilation,
//         ImmutableArray<(INamedTypeSymbol Struct, ImmutableArray<AttributeData> Attribute)> structs)
//     {
//         if (structs.IsDefaultOrEmpty)
//             return;
//
//         var componentType = compilation.GetTypeByMetadataName("Robust.Shared.GameObjects.IComponent");
//         if (componentType == null)
//             return;
//
//         //var allComponents = GetAllComponents(compilation, componentType);
//
//         foreach (var (structSymbol, attribute) in structs)
//         {
//             var source = GenerateSubscriptionClass(compilation, structSymbol, attribute);
//             if (source != null)
//             {
//                 context.AddSource($"{structSymbol.Name}.Subscriptions.g.cs", source);
//             }
//         }
//     }
//
//     private static List<INamedTypeSymbol> GetAllComponents(Compilation compilation, INamedTypeSymbol componentType)
//     {
//         var components = new List<INamedTypeSymbol>();
//         var allTypes = GetAllTypes(compilation.GlobalNamespace);
//
//         foreach (var type in allTypes)
//         {
//             if (type.TypeKind == TypeKind.Class && !type.IsAbstract &&
//                 type.AllInterfaces.Contains(componentType, SymbolEqualityComparer.Default))
//             {
//                 components.Add(type);
//             }
//         }
//
//         return components;
//     }
//
//     private string? GenerateSubscriptionClass(Compilation compilation, INamedTypeSymbol structSymbol,
//         ImmutableArray<AttributeData> attributes)
//     {
//         var sb = new StringBuilder();
//         sb.AppendLine($"namespace {structSymbol.ContainingNamespace.ToDisplayString()}");
//         sb.AppendLine("{");
//         sb.AppendLine("");
//
//         sb.AppendLine("}");
//         return sb.ToString();
//     }
//
//     private void GenerateInner(StringBuilder sb, ITypeSymbol interfaceType)
//     {
//     }
//
//     private void GenerateSubscriptionMethod(StringBuilder sb, ITypeSymbol interfaceType,
//         List<INamedTypeSymbol> components)
//     {
//         var interfaceName = interfaceType.Name.StartsWith("I")
//             ? interfaceType.Name.Substring(1)
//             : interfaceType.Name;
//
//         sb.AppendLine(
//             $"        public void SubscribeAll{interfaceName}<TEvent>(EntityEventRefHandler<{interfaceType.ToDisplayString()}, TEvent> handler) where TEvent : notnull");
//         sb.AppendLine("        {");
//
//         foreach (var component in components)
//         {
//             var componentName = component.Name.Replace(".", "_");
//             sb.AppendLine(
//                 $"            SubscribeLocalEvent<{component.ToDisplayString()}, TEvent>({componentName}Handler);");
//             sb.AppendLine(
//                 $"            void {componentName}Handler(Entity<{component.ToDisplayString()}> ent, ref TEvent ev)");
//             sb.AppendLine($"                => handler((ent.Owner, ent.Comp), ref ev);");
//         }
//
//         sb.AppendLine("        }");
//     }
//
//     private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol root)
//     {
//         foreach (var namespaceOrType in root.GetMembers())
//         {
//             if (namespaceOrType is INamespaceSymbol @namespace)
//             {
//                 foreach (var nested in GetAllTypes(@namespace))
//                 {
//                     yield return nested;
//                 }
//             }
//             else if (namespaceOrType is INamedTypeSymbol type)
//             {
//                 yield return type;
//             }
//         }
//     }
//
//     private static void GenerateInterface(Compilation compilation, INamedTypeSymbol structSymbol,
//         ImmutableArray<AttributeData> attributes)
//     {
//
//     }
//
//     private static void GenerateInnerFields(StringBuilder sb, Compilation compilation, INamedTypeSymbol structSymbol,
//         ImmutableArray<AttributeData> attributes)
//     {
//         sb.AppendLine($"    public partial struct {structSymbol.Name}");
//         sb.AppendLine("    {");
//         var data = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
//         foreach (var attribute in attributes)
//         {
//             if (attribute.AttributeClass?.TypeArguments.Length != 1)
//                 continue;
//             var innerStruct = attribute.AttributeClass.TypeArguments[0];
//             if (!data.Add(innerStruct))
//                 continue; //already added
//             GenerateInner(sb, innerStruct);
//         }
//
//         sb.AppendLine("    }");
//     }
//
//
//     private static string GenerateStaticData()
//     {
//         var sb = new StringBuilder();
//         return sb.ToString();
//     }
// }
