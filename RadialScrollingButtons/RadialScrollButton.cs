
using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace RadialScrollingButtons
{
	/***********************************************************************************
		* Function: 	RadialScrollButton
		* Creator: 		Aylin Opal
		* Description:	Constructor- Creates the buttons.
		* Copyright: 	Hologram LTD 2014.
		***********************************************************************************/
	public class RadialScrollButton : UIButton
	{
		UILabel lblButtonTitle;
		public AppDelegate.enButtons enButton;
		int iButtonCount;
		public RadialScrollButton (float fPosX, int iNewButton) : base(UIButtonType.Custom)
		{
			// Check if iNewButton is in the buttons enum bounds:
			CheckButtonCount (iNewButton);
			enButton = (AppDelegate.enButtons)iButtonCount;
			Frame = new RectangleF (fPosX, 0, AppDelegate.fButtonWidth, AppDelegate.fButtonHeight);
			BackgroundColor = UIColor.Clear;
			lblButtonTitle = new UILabel () {
				Frame = new RectangleF( 0, 0, AppDelegate.fButtonWidth, AppDelegate.fButtonHeight),
				Text = enButton.ToString (),
				TextColor = UIColor.White,
				BackgroundColor = UIColor.Clear,
				TextAlignment = UITextAlignment.Center,
			};
			Add (lblButtonTitle);
			// Alpha = 0 if ContentFadeIn function is used.
			this.Alpha = 0;
		}

		/***********************************************************************************
		* Function: 	CheckButtonCount
		* Creator: 		Aylin Opal
		* Description:	Check if the new buttons count(iNewButton) is greater than the enum count. if so, repeat the buttons.
		* Copyright: 	Hologram LTD 2014.
		***********************************************************************************/
		void CheckButtonCount(int iNew)
		{
			iButtonCount = iNew;
			int iEnumSize = Enum.GetNames (typeof(AppDelegate.enButtons)).Length;
			if (iButtonCount >= iEnumSize) {
				iButtonCount -= iEnumSize;
				CheckButtonCount (iButtonCount);
			}
			else if(iButtonCount < iEnumSize)
				iButtonCount = iNew;
		}

		/***********************************************************************************
		* Function: 	ContentFadeIn
		* Creator: 		Aylin Opal
		* Description:	Fade in function for the buttons.
		* Copyright: 	Hologram LTD 2014.
		***********************************************************************************/
		public void ContentFadeIn(double dDelay, float fAlpha)
		{
			UIView.Animate (0.8f, dDelay, UIViewAnimationOptions.CurveEaseOut , () => { 
				this.Alpha = fAlpha;
			}, () => {
			}
			);
		}
	}
}

