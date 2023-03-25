// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

using NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection;

using Refractions;
using Refractions.Extensions;

using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Graphs;

namespace NatsunekoLaboratory.AnimatorControllerToolPostProcessing
{
    public static class AnimatorControllerToolPostProcessor
    {
        private const string FeatureToggleWriteDefaultsMenu = "NatsunekoLaboratory/Behaviours/Toggle Write Defaults";
        private const string FeatureToggleWriteDefaultsKey = "NatsunekoLaboratory.AnimatorControllerToolPostProcessing.ToggleWriteDefaults";

        private const string FeatureToggleLayerWeightMenu = "NatsunekoLaboratory/Behaviours/Toggle Layer Weight";
        private const string FeatureToggleLayerWeightKey = "NatsunekoLaboratory.AnimatorControllerToolPostProcessing.ToggleLayerWeight";

        private static AnimatorController _previousObj;
        private static int? _previousGraph;
        private static int _previousLayerCount;
        private static int _previousLayerIndex;
        private static int _previousStateCount;

        // ReSharper disable once InconsistentNaming
        private static Refraction<IAnimatorControllerTool> AnimatorControllerTool;

        // ReSharper disable once InconsistentNaming
        private static Refraction<IUndo> Undo;

        // ReSharper disable once InconsistentNaming
        private static Refraction<IStateMachineGraph> StateMachineGraph;

        private static bool IsWriteDefaultsEnabled => EditorPrefs.GetBool(FeatureToggleWriteDefaultsKey, true);

        private static bool IsLayerWeightEnabled => EditorPrefs.GetBool(FeatureToggleLayerWeightKey, true);


        [MenuItem(FeatureToggleWriteDefaultsMenu)]
        private static void ToggleWriteDefaultsBehaviour()
        {
            EditorPrefs.SetBool(FeatureToggleWriteDefaultsKey, !IsWriteDefaultsEnabled);
        }

        [MenuItem(FeatureToggleWriteDefaultsMenu, true)]
        private static bool ValidateWriteDefaultsBehaviourValue()
        {
            Menu.SetChecked(FeatureToggleWriteDefaultsMenu, IsWriteDefaultsEnabled);
            Cleanup();
            return true;
        }

        [MenuItem(FeatureToggleLayerWeightMenu)]
        private static void ToggleLayerWeightBehaviour()
        {
            EditorPrefs.SetBool(FeatureToggleLayerWeightKey, !IsLayerWeightEnabled);
        }

        [MenuItem(FeatureToggleLayerWeightMenu, true)]
        private static bool ValidateLayerWeightBehaviourValue()
        {
            Menu.SetChecked(FeatureToggleLayerWeightMenu, IsLayerWeightEnabled);
            Cleanup();
            return true;
        }

        [InitializeOnLoadMethod]
        private static void OnInitializeOnLoad()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;

            // ReSharper disable once InconsistentNaming
            var UnityEditorGraphs = RefractionResolver.FromType<Graph>();
            AnimatorControllerTool = UnityEditorGraphs.Get<IAnimatorControllerTool>("UnityEditor.Graphs.AnimatorControllerTool");
            StateMachineGraph = UnityEditorGraphs.Get<IStateMachineGraph>("UnityEditor.Graphs.AnimationStateMachine.Graph");

            // ReSharper disable once InconsistentNaming
            var UnityEditorCoreModules = RefractionResolver.FromType<Undo>();
            Undo = UnityEditorCoreModules.Get<IUndo>("UnityEditor.Undo");
        }

        private static void OnUpdate()
        {
            if (!IsWriteDefaultsEnabled && !IsLayerWeightEnabled)
                return; // nop

            var tool = AnimatorControllerTool.ProxyGet(w => w.tool).ToInstantiate(AnimatorControllerTool);
            if (tool.InnerInstance == null)
            {
                Cleanup();
                return;
            }

            var controller = tool.ProxyGet(w => w.m_AnimatorController);
            var graph = tool.ProxyGet(w => w.stateMachineGraph).ToInstantiate(StateMachineGraph);

            if (controller == null || graph.InnerInstance == null)
                return;

            if (controller != _previousObj)
                Setup(tool, controller, graph);

            if (IsLayerWeightEnabled)
                OnHandleLayerAdded(tool, controller);

            if (IsWriteDefaultsEnabled)
                OnHandleStateAdded(tool);

            _previousObj = controller;
            _previousGraph = graph.ProxyInvoke(w => w.GetInstanceID());
            _previousLayerCount = controller.layers.Length;
            _previousLayerIndex = tool.ProxyGet(w => w.selectedLayerIndex);

            var activeStateMachine = graph.InnerInstance == null ? null : graph.ProxyGet(w => w.m_ActiveStateMachine);
            _previousStateCount = activeStateMachine == null ? 0 : activeStateMachine.states.Length;
        }

        private static void Setup(Refraction<IAnimatorControllerTool> tool, AnimatorController controller, Refraction<IStateMachineGraph> graph)
        {
            _previousLayerCount = controller.layers.Length;
            _previousLayerIndex = tool.ProxyGet(w => w.selectedLayerIndex);

            var activeStateMachine = graph.InnerInstance == null ? null : graph.ProxyGet(w => w.m_ActiveStateMachine);
            _previousStateCount = activeStateMachine == null ? 0 : activeStateMachine.states.Length;
        }

        private static void Cleanup()
        {
            _previousObj = null;
            _previousGraph = null;
            _previousLayerCount = -1;
            _previousLayerIndex = -1;
        }

        #region OnHandleStateAdded

        private static void OnHandleStateAdded(Refraction<IAnimatorControllerTool> tool)
        {
            if (tool.ProxyGet(w => w.selectedLayerIndex) != _previousLayerIndex)
                return;

            var graph = tool.ProxyGet(w => w.stateMachineGraph).ToInstantiate(StateMachineGraph);
            if (graph.InnerInstance == null && graph.ProxyInvoke(w => w.GetInstanceID()) != _previousGraph)
                return;

            var activeStateMachine = graph.ProxyGet(w => w.m_ActiveStateMachine);
            var states = activeStateMachine == null ? new ChildAnimatorState[] { } : activeStateMachine.states;
            if (activeStateMachine != null && states.Length > _previousStateCount && IsStateAddedByUser())
                activeStateMachine.states = ModifyNewState(states);
        }

        private static bool IsStateAddedByUser()
        {
            return true; // AnimatorControllerTool#graphDirtyCallback で受け取った方が良いのかもしれないけど、 event じゃない static field なので使いたくない
        }

        private static ChildAnimatorState[] ModifyNewState(ChildAnimatorState[] oldStates)
        {
            var newStates = oldStates;

            foreach (var (oldState, i) in oldStates.Select((w, i) => (w, i)))
            {
                if (i < _previousStateCount)
                {
                    newStates[i] = oldState;
                    continue;
                }

                var newState = oldState;
                newState.state.writeDefaultValues = false;
                newStates[i] = newState;
            }

            return newStates;
        }

        #endregion

        #region OnHandleLayerAdded

        private static void OnHandleLayerAdded(Refraction<IAnimatorControllerTool> tool, AnimatorController controller)
        {
            if (controller.layers.Length > _previousLayerCount && IsLayerAddedByUser(tool, controller))
                controller.layers = ModifyNewLayer(controller.layers);
        }

        private static bool IsLayerAddedByUser(Refraction<IAnimatorControllerTool> tool, AnimatorController controller)
        {
            if (tool.ProxyGet(w => w.selectedLayerIndex) != controller.layers.Length - 1)
                return false;

            var undoRecords = new List<string>();
            var redoRecords = new List<string>();

            Undo.ProxyInvoke(w => w.GetRecords(undoRecords, redoRecords));

            if (undoRecords.Count == 0)
                return false;

            return undoRecords.Last().ToLowerInvariant() == "layer added";
        }

        private static AnimatorControllerLayer[] ModifyNewLayer(AnimatorControllerLayer[] oldLayers)
        {
            var lastLayer = oldLayers.Last();
            lastLayer.defaultWeight = 1.0f;

            var newLayers = oldLayers;
            newLayers[newLayers.Length - 1] = lastLayer;

            return newLayers;
        }

        #endregion
    }
}