// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the License Zero Parity 7.0.0 (see LICENSE-PARITY file) and MIT (contributions, see LICENSE-MIT file) with exception License Zero Patron 1.0.0 (see LICENSE-PATRON file)
// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;

using NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection.Expressions;

using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Graphs;

namespace NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection
{
    public sealed class AnimatorControllerTool : ReflectionClass
    {
        private static readonly Type T;

        public AnimatorController AnimatorController => InvokeField<AnimatorController>("m_AnimatorController", BindingFlags.NonPublic | BindingFlags.Instance);

        public int SelectedLayerIndex => InvokeProperty<int>("selectedLayerIndex", BindingFlags.Public | BindingFlags.Instance);

        public StateMachineGraph StateMachineGraph
        {
            get
            {
                var obj = InvokeField<object>("stateMachineGraph", BindingFlags.Public | BindingFlags.Instance);
                return new StateMachineGraph(obj);
            }
        }

        static AnimatorControllerTool()
        {
            T = typeof(Graph).Assembly.GetType("UnityEditor.Graphs.AnimatorControllerTool");
        }

        private AnimatorControllerTool(object instance) : base(instance, T) { }

        public static AnimatorControllerTool GetCurrentAnimatorControllerTool()
        {
            var t = ReflectionStaticClass.InvokeField<EditorWindow>(T, "tool", BindingFlags.Public | BindingFlags.Static);
            if (t == null)
                return null;

            return new AnimatorControllerTool(t);
        }
    }
}