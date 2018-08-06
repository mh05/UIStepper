using Foundation;
using System;
using CoreGraphics;
using UIKit;
using UIStepController;

namespace UIStepper.Example
{
    public partial class ViewController : UIViewController
    {
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		    var stepper1 = new UIStepperController(new CGRect(113, 150, 150, 32));
            View.AddSubview(stepper1);
		    // Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
    }
}