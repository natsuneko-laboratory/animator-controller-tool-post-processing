// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the License Zero Parity 7.0.0 (see LICENSE-PARITY file) and MIT (contributions, see LICENSE-MIT file) with exception License Zero Patron 1.0.0 (see LICENSE-PATRON file)
// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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