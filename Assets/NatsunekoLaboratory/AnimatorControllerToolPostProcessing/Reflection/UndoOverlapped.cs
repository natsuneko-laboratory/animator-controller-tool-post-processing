// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;

using Refractions.Attributes;

namespace NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection
{
    public interface IUndo
    {
        [NonPublic]
        [Static]
        void GetRecords(List<string> undoRecords, List<string> redoRecords);
    }
}