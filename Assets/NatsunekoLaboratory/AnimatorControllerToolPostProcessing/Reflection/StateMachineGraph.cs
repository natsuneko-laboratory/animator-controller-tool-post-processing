// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Reflection;

using NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection.Expressions;

using UnityEditor.Animations;
using UnityEditor.Graphs;

namespace NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection
{
    public sealed class StateMachineGraph : ReflectionClass
    {
        private static readonly Type T;

        public int InstanceId => RawInstance.GetInstanceID();

        public AnimatorStateMachine ActiveStateMachine => InvokeField<AnimatorStateMachine>("m_ActiveStateMachine", BindingFlags.NonPublic | BindingFlags.Instance);

        static StateMachineGraph()
        {
            T = typeof(Graph).Assembly.GetType("UnityEditor.Graphs.AnimationStateMachine.Graph");
        }

        public StateMachineGraph(object instance) : base(instance, T) { }
    }
}