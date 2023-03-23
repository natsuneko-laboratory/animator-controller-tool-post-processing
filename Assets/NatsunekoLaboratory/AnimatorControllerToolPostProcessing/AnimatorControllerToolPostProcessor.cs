// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Linq;

using NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection;

using UnityEditor;
using UnityEditor.Animations;

namespace NatsunekoLaboratory.AnimatorControllerToolPostProcessing
{
    public static class AnimatorControllerToolPostProcessor
    {
        private const string FeatureToggleWriteDefaultsMenu = "NatsunekoLaboratory/Behaviours/Toggle Write Defaults";
        private const string FeatureToggleWriteDefaultsKey = "NatsunekoLaboratory.AnimatorControllerToolPostProcessing.ToggleWriteDefaults";

        private const string FeatureToggleLayerWeightMenu = "NatsunekoLaboratory/Behaviours/Toggle Layer Weight";
        private const string FeatureToggleLayerWeightKey = "NatsunekoLaboratory.AnimatorControllerToolPostProcessing.ToggleLayerWeight";

        private static AnimatorController _previousObj;
        private static StateMachineGraph _previousGraph;
        private static int _previousLayerCount;
        private static int _previousLayerIndex;
        private static int _previousStateCount;

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
        }

        private static void OnUpdate()
        {
            if (!IsWriteDefaultsEnabled && !IsLayerWeightEnabled)
                return; // nop

            var tool = AnimatorControllerTool.GetCurrentAnimatorControllerTool();
            if (tool == null)
            {
                Cleanup();
                return;
            }

            var controller = tool.AnimatorController;
            var graph = tool.StateMachineGraph;

            if (controller == null || graph == null)
                return;

            if (controller != _previousObj)
                Setup(tool, controller, graph);

            if (IsLayerWeightEnabled)
                OnHandleLayerAdded(tool, controller);

            if (IsWriteDefaultsEnabled)
                OnHandleStateAdded(tool);

            _previousObj = controller;
            _previousGraph = graph;
            _previousLayerCount = controller.layers.Length;
            _previousLayerIndex = tool.SelectedLayerIndex;
            _previousStateCount = graph?.ActiveStateMachine == null ? 0 : graph.ActiveStateMachine.states.Length;
        }

        private static void Setup(AnimatorControllerTool tool, AnimatorController controller, StateMachineGraph graph)
        {
            _previousLayerCount = controller.layers.Length;
            _previousLayerIndex = tool.SelectedLayerIndex;
            _previousStateCount = graph?.ActiveStateMachine == null ? 0 : graph.ActiveStateMachine.states.Length;
        }

        private static void Cleanup()
        {
            _previousObj = null;
            _previousGraph = null;
            _previousLayerCount = -1;
            _previousLayerIndex = -1;
        }

        #region OnHandleStateAdded

        private static void OnHandleStateAdded(AnimatorControllerTool tool)
        {
            if (tool.SelectedLayerIndex != _previousLayerIndex)
                return;

            var graph = tool.StateMachineGraph;
            if (!graph.IsAlive() && graph.InstanceId != _previousGraph.InstanceId)
                return;

            var states = graph.ActiveStateMachine == null ? new ChildAnimatorState[] { } : graph.ActiveStateMachine.states;
            if (graph.ActiveStateMachine != null && states.Length > _previousStateCount && IsStateAddedByUser())
                graph.ActiveStateMachine.states = ModifyNewState(states);
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

        private static void OnHandleLayerAdded(AnimatorControllerTool tool, AnimatorController controller)
        {
            if (controller.layers.Length > _previousLayerCount && IsLayerAddedByUser(tool, controller))
                controller.layers = ModifyNewLayer(controller.layers);
        }

        private static bool IsLayerAddedByUser(AnimatorControllerTool tool, AnimatorController controller)
        {
            if (tool.SelectedLayerIndex != controller.layers.Length - 1)
                return false;

            UndoOverlapped.GetRecords(out var undo, out _);

            return undo.Last() == "Layer Added";
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