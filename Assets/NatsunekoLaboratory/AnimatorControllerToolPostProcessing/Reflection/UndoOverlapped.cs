// -----------------------------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the Microsoft Reference Source License. See LICENSE in the project root for license information.
// -----------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

using NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection.Expressions;

using UnityEditor;

namespace NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection
{
    public static class UndoOverlapped
    {
        private static readonly Type T;

        static UndoOverlapped()
        {
            T = typeof(Undo);
        }

        public static void GetRecords(out List<string> undoRecords, out List<string> redoRecords)
        {
            undoRecords = new List<string>();
            redoRecords = new List<string>();

            ReflectionStaticClass.InvokeMethod(T, "GetRecords", BindingFlags.NonPublic | BindingFlags.Static, undoRecords, redoRecords);
        }
    }
}