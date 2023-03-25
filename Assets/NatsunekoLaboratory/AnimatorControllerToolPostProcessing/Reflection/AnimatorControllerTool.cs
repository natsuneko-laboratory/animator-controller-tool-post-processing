// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Refractions.Attributes;

using UnityEditor.Animations;

// ReSharper disable InconsistentNaming

namespace NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection
{
    public interface IAnimatorControllerTool
    {
        [NonPublic]
        [Instance]
        [Field]
        AnimatorController m_AnimatorController { get; set; }

        [Public]
        [Instance]
        [Field]
        object stateMachineGraph { get; set; }

        [Public]
        [Static]
        [Field]
        object tool { get; set; }

        [Public]
        [Instance]
        int selectedLayerIndex { get; set; }
    }
}