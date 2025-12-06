#if MONOANDROID
using PlatformView = Android.Views.View;
using AndroidX.Activity;
using Android.App;
using Microsoft.Maui.ApplicationModel;
#elif __IOS__ || MACCATALYST
using PlatformView = UIKit.UIView;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.FrameworkElement;
#else
using PlatformView = System.Object;
#endif

namespace Syncfusion.Maui.Toolkit.Internals
{
	/// <summary>
	/// The <see cref="SfWindowOverlay"/> allows the users to add or update an independent 
	/// <see cref="Microsoft.Maui.Controls.View"/> to float above the application window. The AddOrUpdate() methods 
	/// allows you to position it both absolutely and relatively.
	/// The passed view is eliminated from the floating window via the Remove() function. Using the RemoveFromWindow() 
	/// method, you can also delete all floating views. 
	/// </summary>
	internal partial class SfWindowOverlay
	{
		#region Fields

		IWindow? _window;
		bool _hasOverlayStackInRoot = false;
		readonly Dictionary<PlatformView, PositionDetails> _positionDetails;
		WindowOverlayContainer? _overlayStackView;

		// Android-specific back handling: owner assigns a handler that should return true if it handles back press.
		internal Func<bool>? AndroidBackPressedHandler;

	#if MONOANDROID
		AndroidX.Activity.OnBackPressedCallback? _backPressedCallback;
	#endif

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="SfWindowOverlay"/> class.
		/// </summary>
		internal SfWindowOverlay()
		{
			_positionDetails = new Dictionary<PlatformView, PositionDetails>();
		}

		#endregion

		#region Internal Methods

		/// <summary>
		/// Gets the application windows and adds a new overlay stack for including the independent views.
		/// </summary>
		internal void AddToWindow()
		{
			if (!_hasOverlayStackInRoot)
			{
				_window = WindowOverlayHelper._window;
				Initialize();

	#if MONOANDROID
				// Register Android back callback when overlay is added to window
				RegisterAndroidBackCallback();
	#endif
			}
		}

		internal void SetWindowOverlayContainer(WindowOverlayContainer view)
		{
			_overlayStackView = view;
		}
		
		#endregion

		#region Private Methods

	#if MONOANDROID
		void RegisterAndroidBackCallback()
		{
			try
			{
				// Use MAUI's Platform.CurrentActivity to get the current activity
				var activity = Platform.CurrentActivity as Activity;
				if (activity is null)
					return;

				// Already registered
				if (_backPressedCallback != null)
					return;

				_backPressedCallback = new OverlayBackPressedCallback(this, activity);
				activity.OnBackPressedDispatcher.AddCallback(_backPressedCallback);
			}
			catch
			{
				// swallow failures to avoid breaking platforms where registration isn't possible
			}
		}

		// internal helper to let the callback call the owner handler
		internal bool InvokeAndroidBackHandler()
		{
			try
			{
				return AndroidBackPressedHandler?.Invoke() ?? false;
			}
			catch
			{
				return false;
			}
		}

		// Subclass OnBackPressedCallback and route to overlay's handler.
		class OverlayBackPressedCallback : OnBackPressedCallback
		{
			readonly SfWindowOverlay _owner;
			readonly Activity _activity;

			public OverlayBackPressedCallback(SfWindowOverlay owner, Activity activity) : base(true)
			{
				_owner = owner;
				_activity = activity;
			}
			
			public override void HandleOnBackPressed()
			{
				bool handled = _owner.InvokeAndroidBackHandler();

				if (!handled)
				{
					// Not handled by overlay/popup — let the system handle it.
					// Temporarily disable this callback and call default back, avoiding recursion.
					this.Enabled = false;
					_activity.OnBackPressed();
					this.Enabled = true;
				}
				// If handled, we simply return (consume the back).
			}
		}
	#endif

		/// <summary>
		/// Calculates a new absolute position based on the given alignment and size.
		/// </summary>
		/// <param name="horizontalAlignment"></param>
		/// <param name="verticalAlignment"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		void AlignPosition(
			WindowOverlayHorizontalAlignment horizontalAlignment,
			WindowOverlayVerticalAlignment verticalAlignment,
			float width,
			float height,
			ref float x,
			ref float y)
		{
			switch (horizontalAlignment)
			{
				case WindowOverlayHorizontalAlignment.Right:
					x -= width;
					break;

				case WindowOverlayHorizontalAlignment.Center:
					x -= width / 2;
					break;
			}

			switch (verticalAlignment)
			{
				case WindowOverlayVerticalAlignment.Bottom:
					y -= height;
					break;

				case WindowOverlayVerticalAlignment.Center:
					y -= height / 2;
					break;
			}
		}

		/// <summary>
		/// Calculates a new relative position based on the given alignment, relative view size, and child size.
		/// </summary>
		/// <param name="horizontalAlignment"></param>
		/// <param name="verticalAlignment"></param>
		/// <param name="childWidth"></param>
		/// <param name="childHeight"></param>
		/// <param name="relativeViewWidth"></param>
		/// <param name="relativeViewHeight"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		void AlignPositionToRelative(
			WindowOverlayHorizontalAlignment horizontalAlignment,
			WindowOverlayVerticalAlignment verticalAlignment,
			float childWidth,
			float childHeight,
			float relativeViewWidth,
			float relativeViewHeight,
			ref float x,
			ref float y)
		{
			switch (horizontalAlignment)
			{
				case WindowOverlayHorizontalAlignment.Right:
					x += relativeViewWidth;
					break;

				case WindowOverlayHorizontalAlignment.Center:
					x += relativeViewWidth / 2 - childWidth / 2;
					break;

				case WindowOverlayHorizontalAlignment.Left:
					x += -childWidth;
					break;
			}

			switch (verticalAlignment)
			{
				case WindowOverlayVerticalAlignment.Bottom:
					y += relativeViewHeight;
					break;

				case WindowOverlayVerticalAlignment.Center:
					y += relativeViewHeight / 2 - childHeight / 2;
					break;

				case WindowOverlayVerticalAlignment.Top:
					y += -childHeight;
					break;
			}
		}

		#endregion
	}

	/// <summary>
	/// Holds the <see cref="SfWindowOverlay"/> child positioning details for
	/// re-layouting during the window resize and orientation changes.
	/// </summary>
	internal class PositionDetails
	{
		#region Properties

		internal PlatformView? Relative { get; set; }
		
		internal float X { get; set; }
		
		internal float Y { get; set; }
		
		internal WindowOverlayHorizontalAlignment HorizontalAlignment { get; set; }
		
		internal WindowOverlayVerticalAlignment VerticalAlignment { get; set; }
		
		#endregion
	}
}