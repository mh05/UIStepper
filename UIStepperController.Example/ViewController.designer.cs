// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace UIStepper.Example
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIStepController.UIStepperController stepper2 { get; set; }


        [Outlet]
        UIStepController.UIStepperController stepper3 { get; set; }


        [Outlet]
        UIStepController.UIStepperController stepper4 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIStepController.UIStepperController stepper5 { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (stepper2 != null) {
                stepper2.Dispose ();
                stepper2 = null;
            }

            if (stepper3 != null) {
                stepper3.Dispose ();
                stepper3 = null;
            }

            if (stepper4 != null) {
                stepper4.Dispose ();
                stepper4 = null;
            }

            if (stepper5 != null) {
                stepper5.Dispose ();
                stepper5 = null;
            }
        }
    }
}