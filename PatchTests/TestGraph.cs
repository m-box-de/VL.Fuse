﻿using System;
using System.Collections.Generic;
using Fuse;
using Fuse.ShaderFX;
using NUnit.Framework;
using Stride.Core.Mathematics;
using Stride.Graphics;

namespace PatchTests
{
    public static class TestGraph
    {
        [Test]
        public static void TestInputs()
        {
            
            var ShaderNode0 = new GpuInput<float>();
            Console.WriteLine(ShaderNode0.DeclarationList());
            var ShaderNode1 = new GpuInput<float>();
            Console.WriteLine(ShaderNode1.DeclarationList());

            var add = new OperatorNode<float, float>(new List<ShaderNode<float>>
                {ShaderNode0, ShaderNode1, ShaderNode0},new ConstantValue<float>(0),"+");

            var ShaderNode2 = new GpuInput<float>();
            var add2 = new OperatorNode<float,float>(new List<ShaderNode<float>> {add, ShaderNode2},new ConstantValue<float>(0),"+");

            var sin = new IntrinsicFunctionNode<float>(
                new List<AbstractShaderNode> {add, ShaderNode2},
                "sin", new ConstantValue<float>(0));
            Console.WriteLine(sin.BuildSourceCode());
            Console.WriteLine(sin.InputList().Count);
            sin.InputList().ForEach(Console.WriteLine);
        }

        [Test]
        public static void TestDeclarations()
        {
            var ShaderNode0 = new GpuInput<float>();
            var ShaderNode1 = new GpuInput<float>();

            var add = new OperatorNode<float, float>(new List<ShaderNode<float>> {ShaderNode0, ShaderNode1},new ConstantValue<float>(0),"+");

            var ShaderNode2 = new GpuInput<float>();
            var add2 = new OperatorNode<float, float>(new List<ShaderNode<float>> {add, ShaderNode2},new ConstantValue<float>(0),"+");
            Console.WriteLine(add2.DeclarationList());
        }

        [Test]
        public static void TestMixins()
        {
            var ShaderNode0 = new GpuInput<float>();
            
            var customFunction = new MixinFunctionNode<Vector2>(
                new List<AbstractShaderNode>
                {
                    ConstantHelper.FromFloat<Vector2>(0.5f), 
                    ShaderNode0,
                    ShaderNode0
                },
                "voronoise2D", 
                ConstantHelper.FromFloat<Vector2>(0),
                "Voronoise"
            );
            Console.WriteLine(customFunction.MixinList());
            Console.WriteLine(customFunction.BuildSourceCode());
        }

        [Test]
        public static void TestOperatorNode()
        {
            var ShaderNode0 = new GpuInput<float>();
            var ShaderNode1 = new GpuInput<float>();
            var operatorNode = new OperatorNode<float, float>(new List<ShaderNode<float>> {ShaderNode0, ShaderNode1},new ConstantValue<float>(0),"+");
            var operatorNodeWithNull = new OperatorNode<float, float>(new List<ShaderNode<float>> {ShaderNode0, ShaderNode1, null},new ConstantValue<float>(0),"+");
            
            Console.WriteLine(operatorNode.BuildSourceCode());
            Console.WriteLine(operatorNodeWithNull.BuildSourceCode());
        }
        
        [Test]
        public static void TestHallo()
        {
            
            
            Console.WriteLine("HALLO");
        }

        [Test]
        public static void TestCustomFunctions()
        {
            
            var Base = new GpuInput<Vector3>();
            var blend = new GpuInput<Vector3>();
            var opacity = new GpuInput<Vector3>();
            
            var inputs = new List<AbstractShaderNode>
            {
                Base,
                blend,
                opacity,
            };
            const string template = @"
        ${resultType} ${signature}(${resultType} Base, ${resultType} Blend, float Opacity)
        {
            ${resultType} result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
            ${resultType} result2 = 2.0 * Base * Blend;
            ${resultType} zeroOrOne = step(Blend, 0.5);
            ${resultType} Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
            return lerp(Base, Out, Opacity);
        }
";
            
            var customFunction = new CustomFunctionNode<Vector4>(inputs, "blendModeHardLight",template, ConstantHelper.FromFloat<Vector4>(0));
            
            Console.WriteLine(customFunction.BuildSourceCode());
            Console.WriteLine(customFunction.FunctionMap().Count);
            Console.WriteLine(customFunction.Functions.Count);
            customFunction.FunctionMap().ForEach(kv => Console.WriteLine(kv.Value));
            Console.WriteLine(customFunction.FunctionMap());
        }

        [Test]
        public static void TestFloatJoins()
        {
            var ShaderNode0 = new GpuInput<float>();
            var ShaderNode1 = new GpuInput<float>();
            var ShaderNode2 = new GpuInput<float>();
            var ShaderNode3 = new GpuInput<float>();
            
            var join2 = new Float2Join(ShaderNode0, ShaderNode1);
            Console.WriteLine(join2.BuildSourceCode());
            var join2Null = new Float2Join(ShaderNode0, null);
            Console.WriteLine(join2Null.BuildSourceCode());
            
            var join3 = new Float3Join(ShaderNode0, ShaderNode1, ShaderNode2);
            Console.WriteLine(join3.BuildSourceCode());
            var join3Null = new Float3Join(ShaderNode0, ShaderNode1, null);
            Console.WriteLine(join3Null.BuildSourceCode());
            
            var join4 = new Float4Join(ShaderNode0, ShaderNode1, ShaderNode2, ShaderNode3);
            Console.WriteLine(join4.BuildSourceCode());
            var join4Null = new Float4Join(ShaderNode0, ShaderNode1, ShaderNode2, null);
            Console.WriteLine(join4Null.BuildSourceCode());
        }

        [Test]
        public static void TestMember()
        {
            var texCoord = new Semantic<Vector2>("TexCoord");
            var member = new GetMember<Vector2, float>(texCoord, "x");
            Console.WriteLine(member.BuildSourceCode());
            
            var memberNull = new GetMember<Vector2, float>(null, "x");
            Console.WriteLine(memberNull.BuildSourceCode());
        }
        
        [Test]
        public static void TestTraversal()
        {
            var texCoord = new Semantic<Vector2>("TexCoord");
            var ShaderNode0 = new GpuInput<Vector2>();
            var add = new OperatorNode<Vector2, Vector2>(texCoord, ShaderNode0,ConstantHelper.FromFloat<Vector2>(0),"*");

            var memberX = new GetMember<Vector2, float>(add,"x");
            var memberY = new GetMember<Vector2, float>(add,"y");

            var float2Join = new Float2Join(memberX, memberY);
            
            var memberX1 = new GetMember<Vector2, float>(float2Join,"x");
            var memberY1 = new GetMember<Vector2, float>(float2Join,"y");
            
            
            var add1 = new OperatorNode<float, float>(memberX, memberY1,new ConstantValue<float>(0),"+");
            
            Console.WriteLine(add1.BuildSourceCode());
        }

        [Test]
        public static void TestAssign()
        {
            var ShaderNode0 = new GpuInput<Vector2>();
            var ShaderNode1 = new GpuInput<Vector2>();
            var add = new OperatorNode<Vector2, Vector2>(new List<ShaderNode<Vector2>> {ShaderNode0, ShaderNode1},ConstantHelper.FromFloat<Vector2>(0),"+");
            var add2 = new OperatorNode<Vector2, Vector2>(new List<ShaderNode<Vector2>> {ShaderNode0, ShaderNode0},ConstantHelper.FromFloat<Vector2>(0),"+");
            
            var assign = new AssignNode<Vector2>(add, add2);
            
            Console.WriteLine(assign.BuildSourceCode());
        }

        [Test]
        public static void Bits()
        {
            Console.WriteLine(((BufferFlags.UnorderedAccess & BufferFlags.UnorderedAccess) == BufferFlags.UnorderedAccess)+"");
        }

        [Test]
        public static void Main()
        {
            var ShaderNode0 = new GpuInput<float>();
            Console.WriteLine(ShaderNode0.DeclarationList());
            var ShaderNode1 = new GpuInput<float>();
            Console.WriteLine(ShaderNode1.DeclarationList());

            var add = new OperatorNode<float, float>(new List<ShaderNode<float>>
                {ShaderNode0, ShaderNode1, ShaderNode0},new ConstantValue<float>(0),"+");

            var ShaderNode2 = new GpuInput<float>();
            var add2 = new OperatorNode<float,float>(new List<ShaderNode<float>> {add, ShaderNode2},new ConstantValue<float>(0),"+");

            var sin = new IntrinsicFunctionNode<float>(
                new List<AbstractShaderNode> {add, ShaderNode2},
                "sin", new ConstantValue<float>(0));
            Console.WriteLine(add2.BuildSourceCode());
            Console.WriteLine(sin.BuildSourceCode());
            Console.WriteLine(add2.DeclarationList());

            sin.InputList().ForEach(Console.WriteLine);

            var texCoord = new Semantic<Vector2>("TexCoord");
            var add3 = new OperatorNode<Vector2,Vector2>(new List<ShaderNode<Vector2>> {texCoord, texCoord},ConstantHelper.FromFloat<Vector2>(0),"+");
            Console.WriteLine(add3.BuildSourceCode());

            var customFunction = new MixinFunctionNode<Vector2>(
                new List<AbstractShaderNode>
                {
                    ConstantHelper.FromFloat<Vector2>(0.5f), 
                    ShaderNode0,
                    ShaderNode0
                },
                "voronoise2D",
                ConstantHelper.FromFloat<Vector2>(0),
                "Voronoise"
            );
            Console.WriteLine(customFunction.MixinList());
            Console.WriteLine(customFunction.BuildSourceCode());
            
            
        }

        [Test]
        public static void TestToShaderFX()
        {
            var ShaderNode0 = new GpuInput<float>();
            Console.WriteLine(ShaderNode0.DeclarationList());
            var ShaderNode1 = new GpuInput<float>();
            Console.WriteLine(ShaderNode1.DeclarationList());

            var add = new OperatorNode<float, float>(new List<ShaderNode<float>>
                {ShaderNode0, ShaderNode1, ShaderNode0},new ConstantValue<float>(0),"+");

            var ShaderNode2 = new GpuInput<float>();
            var add2 = new OperatorNode<float,float>(new List<ShaderNode<float>> {add, ShaderNode2},new ConstantValue<float>(0),"+");

            var sin = new IntrinsicFunctionNode<float>(
                new List<AbstractShaderNode> {add, ShaderNode2},
                "sin", new ConstantValue<float>(0));

            var toMaterial = new ToShaderFX<float>(null,ShaderNode0);
           Console.WriteLine(toMaterial.ShaderCode);
        }

        [Test]
        public static void TestCreation()
        {
            var _value = new ShaderNode<Vector3>("bla");
            
            var getDeclareBaseType = typeof(DeclareValue<>);
            var dataType = new Type [] { _value.GetType().GetGenericArguments()[0]};
            var getDeclareType = getDeclareBaseType.MakeGenericType(dataType);
            var getDeclareInstance = Activator.CreateInstance(getDeclareType, new object[]{null} ) as AbstractShaderNode;
            Console.WriteLine(getDeclareInstance.GetType().Name);
        }
    }
}