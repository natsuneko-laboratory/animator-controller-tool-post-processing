// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the License Zero Parity 7.0.0 (see LICENSE-PARITY file) and MIT (contributions, see LICENSE-MIT file) with exception License Zero Patron 1.0.0 (see LICENSE-PATRON file)
// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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