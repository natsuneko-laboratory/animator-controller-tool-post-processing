// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Refractions.Attributes;

using UnityEditor.Animations;

// ReSharper disable InconsistentNaming

namespace NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection
{
    public interface IStateMachineGraph
    {
        [Instance]
        [NonPublic]
        [Field]
        AnimatorStateMachine m_ActiveStateMachine { get; set; }

        [Instance]
        [Public]
        int GetInstanceID();
    }
}