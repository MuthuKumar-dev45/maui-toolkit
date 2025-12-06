// Copyright and usings preserved from original file
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Syncfusion.Maui.Toolkit.Internals;
using Syncfusion.Maui.Toolkit.Themes;
using MauiView = Microsoft.Maui.Controls.View;

namespace Syncfusion.Maui.Toolkit.Popup
{
	/// <summary>
	/// <see cref="SfPopup"/> displays an alert message with customizable buttons or loads a desired view within a pop-up window.
	/// </summary>
	public partial class SfPopup : SfView, IParentThemeElement
	{
		#region Fields

		/// <summary>
		/// The overlay of the _popupView.
		/// </summary>
		internal SfWindowOverlay? _popupOverlay;

		/// <summary>
		/// The popup overlay container view of the _popupView.
		/// </summary>
		internal SfPopupOverlayContainer? _popupOverlayContainer;

		/// <summary>
		/// The popup view of the <see cref="SfPopup"/> that will be displayed when setting the <see cref="SfPopup.IsOpen"/> property as <c>true</c>.
		/// </summary>
		internal PopupView? _popupView;

		/// <summary>
		/// Default height of the _popupView.
		/// </summary>
		internal double _popupViewHeight;

		/// <summary>
		/// Default body height of the PopupView.
		/// </summary>
		internal double _defaultPopupViewHeight;

		/// <summary>
		/// Default width of the _popupView.
		/// </summary>
		internal double _popupViewWidth;

		/// <summary>
		/// Default width of the PopupView.
		/// </summary>
		internal double _defaultPopupViewWidth = 313;

		/// <summary>
		/// Backing field to store the corner radius value.
		/// </summary>
		internal double _radiusValue = 0;

		/// <summary>
		/// Boolean value indicating whether the Popup opening or closing animation is in progress.
		/// </summary>
		internal bool _isPopupAnimationInProgress;

		/// <summary>
		/// Boolean value indicating whether the container opening or closing animation is in progress.
		/// </summary>
		internal bool _isContainerAnimationInProgress;

		/// <summary>
		/// Gets a value indicating whether the flow direction is RTL or not.
		/// </summary>
		internal bool _isRTL = false;

		/// <summary>
		/// Gets a value indicating whether the IsOpen is in progress.
		/// </summary>
		internal bool _isOpenInProgress = false;

		/// <summary>
		/// Represents a task completion source that can be used to create and control a Tasks with a result of type bool.
		/// </summary>
		TaskCompletionSource<bool>? _taskCompletionSource;

		/// <summary>
		/// X-point of the Popup, after calculation of the popup view x position.
		/// </summary>
		double _popupXPosition;

		/// <summary>
		/// Y-point of the Popup, after calculation of the popup view y position.
		/// </summary>
		double _popupYPosition;

		/// <summary>
		/// View relative to which popup should be displayed.
		/// </summary>
		View? _relativeView;

		/// <summary>
		/// Position relative to the RelativeView, from which popup should be displayed.
		/// </summary>
		PopupRelativePosition _relativePosition;

		/// <summary>
		/// absolute X-Point where the popup should be positioned from the relative view.
		/// </summary>
		double _absoluteXPoint;

		/// <summary>
		/// absolute Y-Point where the popup should be positioned from the relative view.
		/// </summary>
		double _absoluteYPoint;

		/// <summary>
		/// Boolean value indicating whether the popup can be shown in fullscreen or not.
		/// </summary>
		bool _showFullScreen;

		/// <summary>
		/// Backing field for the <see cref="AppliedHeaderHeight"/> property.
		/// </summary>
		double _appliedHeaderHeight;

		/// <summary>
		/// Backing field for the <see cref="AppliedFooterHeight"/> property.
		/// </summary>
		double _appliedFooterHeight;

		/// <summary>
		/// Backing field for the <see cref="AppliedBodyHeight"/> property.
		/// </summary>
		double _appliedBodyHeight;

		/// <summary>
		/// Backing field for the <see cref="AppliedPopupBodyWidth"/> property.
		/// </summary>
		double _appliedPopupBodyWidth;

		/// <summary>
		/// At Show(x,y) given x point in Sample to display the Popup.
		/// </summary>
		double _showXPosition;

		/// <summary>
		/// At Show(x,y) given y point in Sample to display the Popup.
		/// </summary>
		double _showYPosition;

		/// <summary>
		/// Minimal padding value used to identify whether _popupView is positioned at screen edges.
		/// </summary>
		int _minimalPadding = 0;

		/// <summary>
		///  Backing field for SemanticDescription.
		/// </summary>
		string? _semanticDescription;

		/// <summary>
		///  Backing field for CanShowPopupInFullScreen.
		/// </summary>
		bool _canShowPopupInFullScreen;

		/// <summary>
		///  Field for to avoid Opening the Popup again, after canceling the Closing event.
		/// </summary>
		bool _canOpenPopup = true;

		/// <summary>
		/// Field to store keyboard height.
		/// </summary>
		double _keyboardHeight = 0;

		/// <summary>
		/// Backing field for the <see cref="ScreenHeight"/> property.
		/// </summary>
		int _screenHeight;

		/// <summary>
		/// Backing field for the <see cref="ScreenWidth"/> property.
		/// </summary>
		int _screenWidth;

		/// <summary>
		/// Backing field for the <see cref="StatusBarHeight"/> property.
		/// </summary>
		double _statusBarHeight;

		/// <summary>
		/// Backing field for the <see cref="SafeAreaAtLeft"/> property.
		/// </summary>
		int _safeAreaAtLeft;

		/// <summary>
		/// Backing field for the <see cref="SafeAreaAtRight"/> property.
		/// </summary>
		int _safeAreaAtRight;

		/// <summary>
		/// Backing field for the <see cref="SafeAreaAtTop"/> property.
		/// </summary>
		int _safeAreaAtTop;

		/// <summary>
		/// Backing field for the <see cref="SafeAreaAtBottom"/> property.
		/// </summary>
		int _safeAreaAtBottom;

		/// <summary>
		/// Indicates if the AddToOverlay operation was successful.
		/// </summary>
		bool _isOverlayAdded;

		/// <summary>
		/// Indicates if opening the popup is deferred until certain conditions are met.
		/// </summary>
		bool _isOpenDeferred;

		#endregion

		#region Bindable Properties

		// ... existing bindable properties ...

		/// <summary>
		/// Identifies the ClosePopupOnBackButtonPressed bindable property.
		/// When true (default) pressing Android Back will close the popup (if allowed).
		/// When false, Android Back will not close the popup.
		/// </summary>
		public static readonly BindableProperty ClosePopupOnBackButtonPressedProperty =
			BindableProperty.Create(nameof(ClosePopupOnBackButtonPressed), typeof(bool), typeof(SfPopup), true, BindingMode.Default);

		/// <summary>
		/// Gets or sets whether the popup should be closed when the Android back button is pressed.
		/// Default: true (popup will be closed).
		/// Set to false to prevent the popup from closing on Android back.
		/// </summary>
		public bool ClosePopupOnBackButtonPressed
		{
			get => (bool)GetValue(ClosePopupOnBackButtonPressedProperty);
			set => SetValue(ClosePopupOnBackButtonPressedProperty, value);
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="SfPopup"/> class.
		/// </summary>
		public SfPopup()
		{
			Initialize();
			ThemeElement.InitializeThemeResources(this, "SfPopupTheme");
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Initializes the properties for <see cref="SfPopup"/>.
		/// </summary>
		void Initialize()
		{
			SfPopupResources.InitializeDefaultResource("Syncfusion.Maui.Toolkit.Popup.Resources.SfPopup", typeof(SfPopup));
			SetPopupViewDefaultHeight();
			_showXPosition = -1;
			_showYPosition = -1;
			_popupView = new PopupView(this);
			_popupOverlay = new SfWindowOverlay();
			_popupOverlayContainer = new SfPopupOverlayContainer(this);
			_popupOverlay.SetWindowOverlayContainer(_popupOverlayContainer);

			// Register Android back-button handler on overlay so back press can close popup without requiring MainActivity changes.
			// Handler returns true if it handled the back press (popup closed), false to allow default back behavior.
			_popupOverlay.AndroidBackPressedHandler = () =>
			{
				// If the consumer disabled close on back, do not handle the back button here.
				if (!ClosePopupOnBackButtonPressed)
				{
					return false;
				}

				// Only handle back when popup is open.
				if (IsOpen)
				{
					// If popup should stay open, do not handle back here.
					if (StaysOpen)
					{
						return false;
					}

					// Close the popup and indicate the back was handled.
					IsOpen = false;
					return true;
				}

				return false;
			};

			SetPopupPositionBasedOnKeyboard();
			InitializeOverlay();
			if (!HeaderTitle.Equals("Title", StringComparison.Ordinal))
			{
				_popupView.SetHeaderTitleText(HeaderTitle);
			}

			if (!Message.Equals("Popup Message", StringComparison.Ordinal))
			{
				_popupView.SetMessageText(Message);
			}

			if (!AcceptButtonText.Equals("ACCEPT", StringComparison.Ordinal))
			{
				_popupView.SetAcceptButtonText(AcceptButtonText);
			}

			if (!DeclineButtonText.Equals("DECLINE", StringComparison.Ordinal))
			{
				_popupView.SetDeclineButtonText(DeclineButtonText);
			}
		}

		// ... rest of original file remains unchanged ...
	}