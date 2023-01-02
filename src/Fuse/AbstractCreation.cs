﻿using System;
using Fuse.compute;
using Stride.Graphics;

namespace Fuse
{
    public static class AbstractCreation
    {
        public static AbstractShaderNode CreateAbstract(AbstractShaderNode theValue, Type theBaseType, object[] theArguments )
        {
            var nodeType = theValue.GetType();

            while (nodeType != null && nodeType.BaseType != null && nodeType.BaseType != typeof(AbstractShaderNode))
            {
                nodeType = nodeType.BaseType;
            }
            
            var dataType = new[] { nodeType.GetGenericArguments()[0]};
            var getType = theBaseType.MakeGenericType(dataType);
            return Activator.CreateInstance(getType, theArguments) as AbstractShaderNode;
        }

        private static Type GetBaseType(AbstractShaderNode theNode)
        {
            var nodeType = theNode.GetType();
            while (nodeType is {BaseType: { }} && nodeType.BaseType != typeof(AbstractShaderNode))
            {
                nodeType = nodeType.BaseType;
            }

            return nodeType;
        }
        
        public static AbstractShaderNode AbstractGetMember<T>(NodeSubContextFactory theSubContextFactory, ShaderNode<T> theStruct, AbstractShaderNode theMember)
        {
            //return CreateAbstract(theMember, typeof(GetMember<,>), new object[]{theSubContextFactory.NextSubContext(), theStruct, theMember.Name, null});
            
           
            var getMemberBaseType = typeof(GetMember<,>);
            var nodeType = GetBaseType(theMember);
            var dataType = new[] {typeof(T), nodeType.GetGenericArguments()[0]};
            var getType = getMemberBaseType.MakeGenericType(dataType);
            return Activator.CreateInstance(getType, theSubContextFactory.NextSubContext(), theStruct, theMember.Name, null) as AbstractShaderNode;
          
        }
        
        public static AbstractShaderNode AbstractSetMember<T>(NodeSubContextFactory theSubContextFactory, ShaderNode<T> theStruct, string theMember, AbstractShaderNode theValue)
        {
            //return CreateAbstract(theMember, typeof(AssignValueToMember<>), new object[]{theSubContextFactory.NextSubContext(), theStruct, theMember.Name, theValue});
            
            
            var setMemberBaseType = typeof(AssignValueToMember<>);
            var dataType = new[] {typeof(T)};
            var getType = setMemberBaseType.MakeGenericType(dataType);
            return Activator.CreateInstance(getType, theSubContextFactory.NextSubContext(), theStruct, theMember, theValue) as AbstractShaderNode;
           
        }
        
        public static AbstractShaderNode AbstractComputeTextureGet(NodeSubContextFactory theSubContextFactory, ShaderNode<Texture> theTexture, AbstractShaderNode theIndex, AbstractShaderNode theValue)
        {
            
            var indexType = GetBaseType(theIndex);
            var valueType = GetBaseType(theValue);
            
            var baseType = typeof(ComputeTextureGet<,>);
            var dataType = new[] {indexType.GetGenericArguments()[0], valueType.GetGenericArguments()[0]};
            var getType = baseType.MakeGenericType(dataType);
            return Activator.CreateInstance(getType, theSubContextFactory.NextSubContext(), theTexture, theIndex, null) as AbstractShaderNode;
        }
        
        public static AbstractShaderNode AbstractComputeTextureSet(NodeSubContextFactory theSubContextFactory, ShaderNode<Texture> theTexture, AbstractShaderNode theIndex, AbstractShaderNode theValue)
        {
            
            var indexType = GetBaseType(theIndex);
            var valueType = GetBaseType(theValue);
            
            var baseType = typeof(ComputeTextureSet<,>);
            var dataType = new[] {indexType.GetGenericArguments()[0], valueType.GetGenericArguments()[0]};
            var getType = baseType.MakeGenericType(dataType);
            return Activator.CreateInstance(getType, theSubContextFactory.NextSubContext(), theTexture, theIndex, theValue) as AbstractShaderNode;
        }
        
        public static AbstractShaderNode AbstractShaderNodePassThrough(NodeSubContextFactory theSubContextFactory, AbstractShaderNode theValue)
        {
            return CreateAbstract(theValue, typeof(PassThroughNode<>), new object[]{theSubContextFactory.NextSubContext(), "pass", theValue});
            /*
            var getBaseType = typeof(PassThroughNode<>);
            var nodeType = theValue.GetType().BaseType;
            if (nodeType == null) return null;
            var dataType = new[] { nodeType.GetGenericArguments()[0] };
            var getType = getBaseType.MakeGenericType(dataType);
            return Activator.CreateInstance(getType, theValue) as AbstractShaderNode;
            */
        }
        
        public static AbstractShaderNode AbstractDeclareValue(NodeSubContextFactory theSubContextFactory, AbstractShaderNode theValue)
        {
            return CreateAbstract(theValue, typeof(DeclareValue<>), new object[]{theSubContextFactory.NextSubContext()});
        }
        
        public static AbstractShaderNode AbstractDeclareValueAssigned(NodeSubContextFactory theSubContextFactory, AbstractShaderNode theValue)
        {
            return CreateAbstract(theValue, typeof(DeclareValue<>), new object[] {theSubContextFactory.NextSubContext(), theValue});
        }

        public static void CallFunction(AbstractShaderNode theNode, string theFunctionName, object[] theArgs)
        {
            var resultType = theNode.GetType();
            var method = resultType.GetMethod(theFunctionName);
            method?.Invoke(theNode, theArgs);
        }
        
        public static AbstractShaderNode AbstractOutput(NodeSubContextFactory theSubContextFactory, AbstractShaderNode theComputation, AbstractShaderNode theOutput)
        {
            var result = CreateAbstract(theOutput, typeof(Output<>), new object[] {theSubContextFactory.NextSubContext(), null});
            CallFunction(result,"SetInputsAbstract",new object[]{ theComputation, theOutput});
            return result;
        }
        
        public static AbstractShaderNode AbstractAssignNode(NodeSubContextFactory theSubContextFactory, AbstractShaderNode theTarget, AbstractShaderNode theSource)
        {
            return CreateAbstract(theTarget, typeof(AssignValue<>), new object[] {theSubContextFactory.NextSubContext(), theTarget, theSource});
        }
        
        public static AbstractShaderNode AbstractConstant(NodeSubContextFactory theSubContextFactory, AbstractShaderNode theGpuValue, float theValue)
        {
            var dataType = new[] { theGpuValue.GetType().BaseType.GetGenericArguments()[0]};
            return ConstantHelper.AbstractFromFloat(theSubContextFactory.NextSubContext(), dataType[0], theValue);
        }
        
        
    }
}