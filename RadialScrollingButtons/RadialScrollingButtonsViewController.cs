using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Collections.Generic;

namespace RadialScrollingButtons
{

	/***********************************************************************************
	* Class: 		RadialScrollingButtonsViewController
	* Creator: 		Aylin Opal
	* Description:	View controller for the radial widget
	* Copyright: 	Hologram LTD 2014.
	***********************************************************************************/
	public partial class RadialScrollingButtonsViewController : UIViewController
	{
		UIView vTouchDetector;
		PointF pCurrentTouchPoint;

		//Buttons X distance from the center. 
		float fButtonXPosition = 0;
		//Rotation speed:
		float fRotateAmount = 2;
		UIView vRotatingContainer;
		float fStartingAngle = -60;
		//Number of buttons:
		public int iButtonCount = 6;
		UIPanGestureRecognizer pgGesture;
		UIView[] vaButtonContainers;
		List<RadialScrollButton> lrButtons;
		public RadialScrollingButtonsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.

			// uiview to detect touches:
			vTouchDetector = new UIView (new RectangleF (0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height));
			vTouchDetector.BackgroundColor = UIColor.Clear;

			// The rotating container for the buttons. 
			vRotatingContainer = new UIView (new RectangleF (0, 0, AppDelegate.fButtonWidth*4, AppDelegate.fButtonWidth*4));
			vRotatingContainer.Layer.AnchorPoint = new PointF (0.5f,0.5f);
			vRotatingContainer.BackgroundColor = UIColor.Clear;
			// Center the container:
			vRotatingContainer.Center = new PointF (UIScreen.MainScreen.Bounds.Width/2, UIScreen.MainScreen.Bounds.Height/2 );

			//Container array. a uiview for each button:
			vaButtonContainers = new UIView[iButtonCount];
			lrButtons = new List< RadialScrollButton>();
			for(int i = 0; i < iButtonCount; i++)
			{
				// Set the size:
				vaButtonContainers[i] = new UIView (new RectangleF (0, 0, AppDelegate.fButtonWidth*2, AppDelegate.fButtonHeight));
				//Set anchor point:
				vaButtonContainers [i].Layer.AnchorPoint = new PointF (0f,0.5f);
				//Center the containers according to the parent rotating container view:
				vaButtonContainers [i].Center = new PointF (vRotatingContainer.Frame.Width/2, vRotatingContainer.Frame.Height/2);
//				if (i % 2 == 0)
//					vaButtonContainers [i].BackgroundColor = UIColor.LightGray;
//				else
//					vaButtonContainers [i].BackgroundColor = UIColor.Purple;
				//Create a button:
				RadialScrollButton rbButton = new RadialScrollButton (fButtonXPosition ,i);
				rbButton.SetBackgroundImage (UIImage.FromBundle ("Button.png"), UIControlState.Normal);
		
				rbButton.TouchUpInside += (sender, e) => 
				{
					Console.WriteLine("touch!");
					AppDelegate.enSelectedButton = rbButton.enButton;
				};
				rbButton.Layer.AnchorPoint = new PointF (0, 0.5f);
				vaButtonContainers[i].Transform = CGAffineTransform.MakeRotation ((float)Math.PI * i*(360/iButtonCount)/180);
				lrButtons.Add(rbButton);
				//Add button to button container, and button container to rotating container.
				vaButtonContainers[i].Add (rbButton);

				vRotatingContainer.Add (vaButtonContainers [i]);

			}
			vRotatingContainer.Transform = CGAffineTransform.MakeRotation ((float)Math.PI *(fStartingAngle)/180);
			vRotatingContainer.UserInteractionEnabled = true;
			vTouchDetector.Add (vRotatingContainer);

			Add (vTouchDetector);
			// Set gesture recognizer:
			vTouchDetector.UserInteractionEnabled = true;
			pgGesture = new UIPanGestureRecognizer ();
			pgGesture.AddTarget (() => { 
				HandlePan(pgGesture); });
			vTouchDetector.AddGestureRecognizer (pgGesture);
			pgGesture.CancelsTouchesInView = false;

		}
		bool bSwipeUp = false, bSwipeDown = false, bSwipeLeft = false, bSwipeRight = false;

		int iX0;
		int iY0;
		/***********************************************************************************
		* Function: 	HandlePan
		* Creator: 		Aylin Opal
		* Description:	Pan event handler. Calls the RotateButtons function for each pan event.
		* Copyright: 	Hologram LTD 2014.
		***********************************************************************************/
		void HandlePan(UIPanGestureRecognizer sRecognizer)
		{
			Console.WriteLine ("Handle Pan");

			pCurrentTouchPoint = sRecognizer.LocationInView (vTouchDetector);
			//Get swipe direction: 
			//http://stackoverflow.com/questions/19037619/how-to-set-direction-of-uipangesturerecognizer-in-xamarin-monotouch
			PointF pTranslation = sRecognizer.TranslationInView (vTouchDetector);
			int iX = Convert.ToInt32( pTranslation.X);
			int iY = Convert.ToInt32 (pTranslation.Y);

			int absX = Math.Abs(Math.Abs(iX) - Math.Abs(iX0));
			int absY = Math.Abs(Math.Abs(iY) - Math.Abs(iY0));
				
			bool bHorizontal, bVertical;
			bHorizontal = (absX > absY);
			bVertical = !bHorizontal;

			bSwipeLeft = (bHorizontal && iX0 > iX);
			bSwipeRight = (bHorizontal && iX0 < iX); 
			bSwipeUp = (bVertical && iY0 > iY);
			bSwipeDown = (bVertical && iY0 < iY);

			//swiping horizontally:
			//if touch is on the lower half of the screen:
			if (bHorizontal) {
				Console.WriteLine ("Horizontal");
			
				if (pCurrentTouchPoint.Y > UIScreen.MainScreen.Bounds.Height / 2)   {
				if (bSwipeLeft) {
//						Console.WriteLine ("Horizontal - > - LEFT");
					RotateButtons (fRotateAmount);
				} else if (bSwipeRight) {
//						Console.WriteLine ("Horizontal - > - RIGHT");
					RotateButtons (-fRotateAmount);
				}
				} else {
					if (bSwipeLeft) {
//						Console.WriteLine ("Horizontal - < - LEFT");
						RotateButtons (-fRotateAmount);
					} else if (bSwipeRight) {
//						Console.WriteLine ("Horizontal - < - RIGHT");
						RotateButtons (fRotateAmount);
					}
				}
			}
			//Swiping vertically:
			//if touch is on the right side of the screen:
			else if (bVertical) {
				Console.WriteLine ("Vertical");
			
				if (pCurrentTouchPoint.X > UIScreen.MainScreen.Bounds.Width / 2) {
					if (bSwipeUp) {
						RotateButtons (-fRotateAmount);
					} else if (bSwipeDown) {
						RotateButtons (fRotateAmount);
					}
				} else {
					if (bSwipeUp) {
						RotateButtons (fRotateAmount);
					} else if (bSwipeDown) {
						RotateButtons (-fRotateAmount);
					}
				}
			}
			iX0 = iX;
			iY0 = iY;
		}

		/***********************************************************************************
		* Function: 	RotateButtons
		* Creator: 		Aylin Opal
		* Description:	Rotates the buttons container.
		* Copyright: 	Hologram LTD 2014.
		***********************************************************************************/
		void RotateButtons(float fSwipeAmount)
		{
			// Get current rotation in radians:
			double dCurrentRotRadian = Math.Atan2 (Convert.ToDouble( vRotatingContainer.Transform.yx), Convert.ToDouble( vRotatingContainer.Transform.yy));
			// Convert current rotation to degrees:
			float fCurrentRot =(float)( dCurrentRotRadian * (180 / Math.PI));
			// Rotate:
			UIView.Animate (0.1f, 0, UIViewAnimationOptions.CurveEaseIn, () => { 
				vRotatingContainer.Transform = CGAffineTransform.MakeRotation ((float)Math.PI * (fCurrentRot + fSwipeAmount) / 180);
	
			}, () => {
			
				}
				);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			for (int i = 0; i < iButtonCount; i++) {
				(lrButtons[i]).ContentFadeIn (i * 0.3f, 1);
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}
		#endregion
	}
}