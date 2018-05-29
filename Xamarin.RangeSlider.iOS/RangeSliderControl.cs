using System;
using System.ComponentModel;
using System.Globalization;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.RangeSlider.Common;

namespace Xamarin.RangeSlider
{
    [Preserve(AllMembers = true)]
    [DesignTimeVisible(true)]
    public partial class RangeSliderControl : UIControl
    {
        public const int TextLateralPadding = 5;

        public const int BarHeight = 2;

        private UIImageView _lowerHandle;

        private bool _lowerHandleHidden;

        private float _lowerHandleHiddenWidth;

        // Images, these should be set before the control is displayed.
        // If they are not set, then the default images are used.
        // eg viewDidLoad

        //Probably should add support for all control states... Anyone?

        private UIImage _lowerHandleImage;

        // maximum value for left thumb
        private float _lowerMaximumValue;

        private UIEdgeInsets _lowerTouchEdgeInsets;

        private float _lowerTouchOffset;
        private float _stepValueInternal;

        private UIView _track;
        private UIView _trackBackground;

        private UIImageView _upperHandle;
        private bool _upperHandleHidden;
        private float _upperHandleHiddenWidth;

        private UIImage _upperHandleImage;

        /// <summary>
        ///     minimum value for right thumb
        /// </summary>
        private float _upperMinimumValue;

        private UIEdgeInsets _upperTouchEdgeInsets;
        private float _upperTouchOffset;

        private UILabel _lowerHandleLabel;
        private UILabel _upperHandleLabel;
        private bool _showTextAboveThumbs;
        private float _textSize = 10;
        private string _textFormat = "F0";
        private UIColor _textColor;
        private float _lowerValue;
        private float _maximumValue;
        private float _minimumRange;
        private float _minimumValue;
        private bool _continuous;
        private float _stepValue;
        private bool _stepValueContinuously;
        private float _upperValue;


        public RangeSliderControl()
        {
            ConfigureView();
        }

        public RangeSliderControl(IntPtr handle) : base(handle)
        {
            ConfigureView();
        }

        public RangeSliderControl(NSCoder coder) : base(coder)
        {
            ConfigureView();
        }

        public RangeSliderControl(CGRect frame) : base(frame)
        {
            ConfigureView();
        }

        /// <summary>
        ///     defafult true, indicating whether changes in the sliders value generate continuous update events.
        /// </summary>
        [Export("Continuous")]
        [Browsable(true)]
        public bool Continuous
        {
            get { return _continuous; }
            set
            {
                _continuous = value;
                SetNeedsLayout();
            }
        }

        /// <summary>
        ///     default 0.0. this value will be pinned to min/max
        /// </summary>
        [Export("LowerValue")]
        [Browsable(true)]
        public float LowerValue
        {
            get { return _lowerValue; }
            set
            {
                _lowerValue = value;
                SetNeedsLayout();
            }
        }

        /// <summary>
        ///     default 1.0
        /// </summary>
        [Export("MaximumValue")]
        [Browsable(true)]
        public float MaximumValue
        {
            get { return _maximumValue; }
            set
            {
                _maximumValue = value;
                SetNeedsLayout();
            }
        }

        /// <summary>
        ///     default 0.0. This is the minimum distance between between the upper and lower values
        /// </summary>
        [Export("MinimumRange")]
        [Browsable(true)]
        public float MinimumRange
        {
            get { return _minimumRange; }
            set
            {
                _minimumRange = value;
                SetNeedsLayout();
            }
        }

        [Export("MinimumValue")]
        [Browsable(true)]
        public float MinimumValue
        {
            get { return _minimumValue; }
            set
            {
                _minimumValue = value;
                SetNeedsLayout();
            }
        }

        /// <summary>
        ///     default 0.0 (disabled)
        /// </summary>
        [Export("StepValue")]
        [Browsable(true)]
        public float StepValue
        {
            get { return _stepValue; }
            set
            {
                _stepValue = value;
                SetNeedsLayout();
            }
        }

        /// <summary>
        /// If false the slider will move freely with the tounch. When the touch ends, the value will snap to the nearest step value
        /// If true the slider will stay in its current position until it reaches a new step value.
        /// default false
        /// </summary>
        [Export("StepValueContinuously")]
        [Browsable(true)]
        public bool StepValueContinuously
        {
            get { return _stepValueContinuously; }
            set
            {
                _stepValueContinuously = value;
                SetNeedsLayout();
            }
        }

        /// <summary>
        ///     default 1.0. this value will be pinned to min/max
        /// </summary>
        [Export("UpperValue")]
        [Browsable(true)]
        public float UpperValue
        {
            get { return _upperValue; }
            set
            {
                _upperValue = value;
                SetNeedsLayout();
            }
        }

        [Export("LowerHandleHidden")]
        [Browsable(true)]
        public bool LowerHandleHidden
        {
            get { return _lowerHandleHidden; }
            set
            {
                _lowerHandleHidden = value;
                SetNeedsLayout();
            }
        }

        [Export("UpperHandleHidden")]
        [Browsable(true)]
        public bool UpperHandleHidden
        {
            get { return _upperHandleHidden; }
            set
            {
                _upperHandleHidden = value;
                SetNeedsLayout();
            }
        }

        [Export("TextSize")]
        [Browsable(true)]
        public float TextSize
        {
            get { return _textSize; }
            set
            {
                _textSize = value;
                _lowerHandleLabel.Font = UIFont.SystemFontOfSize(_textSize);
                _upperHandleLabel.Font = UIFont.SystemFontOfSize(_textSize);
                InvalidateIntrinsicContentSize();
                Frame = new CGRect(new CGPoint(0, 0), IntrinsicContentSize);
                SetNeedsLayout();
            }
        }

        [Export("ShowTextAboveThumbs")]
        [Browsable(true)]
        public bool ShowTextAboveThumbs
        {
            get { return _showTextAboveThumbs; }
            set
            {
                _showTextAboveThumbs = value;
                InvalidateIntrinsicContentSize();
                Frame = new CGRect(new CGPoint(0, 0), IntrinsicContentSize);
                SetNeedsLayout();
            }
        }

        [Export("TextFormat")]
        [Browsable(true)]
        public string TextFormat
        {
            get { return _textFormat; }
            set
            {
                _textFormat = string.IsNullOrWhiteSpace(value) ? "F0" : value;
                SetNeedsLayout();
            }
        }

        [Export("TextColor")]
        [Browsable(true)]
        public UIColor TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                SetNeedsLayout();
            }
        }

        public Func<Thumb, float, string> FormatLabel { get; set; }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                _lowerHandleImage = null;
                _upperHandleImage = null;
                _track.Alpha = Enabled ? 1f : 0.2f;
                SetNeedsLayout();
            }
        }

        private UIImage LowerHandleImage
        {
            get
            {
                if (_lowerHandleImage != null)
                    return _lowerHandleImage;

                var image = ImageFromBundle(Enabled ? @"slider-default-handle" : @"slider-default-handle-disabled");
                _lowerHandleImage = image.ImageWithAlignmentRectInsets(new UIEdgeInsets(-1, 8, 1, 8));

                return _lowerHandleImage;
            }
        }

        private UIImage UpperHandleImage
        {
            get
            {
                if (_upperHandleImage != null)
                    return _upperHandleImage;

                var image = ImageFromBundle(Enabled ? @"slider-default-handle" : @"slider-default-handle-disabled");
                _upperHandleImage = image.ImageWithAlignmentRectInsets(new UIEdgeInsets(-1, 8, 1, 8));

                return _upperHandleImage;
            }
        }

        public override CGSize IntrinsicContentSize => new CGSize(NoIntrinsicMetric,
            Math.Max(LowerHandleImage.Size.Height, UpperHandleImage.Size.Height) + SpaceAboveThumbs);

        private nfloat SpaceAboveThumbs
            => ShowTextAboveThumbs ? GetTextAboveThumbSize(_lowerHandleLabel).Height : 0;

        public event EventHandler LowerValueChanged;
        public event EventHandler UpperValueChanged;
        public event EventHandler DragStarted;
        public event EventHandler DragCompleted;

        private void ConfigureView()
        {
            //Setup the default values
            MinimumValue = 0f;
            MaximumValue = 1f;
            MinimumRange = 0f;
            StepValue = 0f;
            _stepValueInternal = 0.0f;

            Continuous = true;

            LowerValue = MinimumValue;
            UpperValue = MaximumValue;

            _lowerMaximumValue = float.NaN;
            _upperMinimumValue = float.NaN;
            _upperHandleHidden = false;
            _lowerHandleHidden = false;

            _lowerHandleHiddenWidth = 2.0f;
            _upperHandleHiddenWidth = 2.0f;

            _lowerTouchEdgeInsets = new UIEdgeInsets(-5, -5, -5, -5);
            _upperTouchEdgeInsets = new UIEdgeInsets(-5, -5, -5, -5);

            AddSubviews();
        }

        private void SetLowerValue(float lowerValue)
        {
            var value = lowerValue;

            if (_stepValueInternal > 0)
            {
                value = RoundToStepValue(value);
            }

            value = Math.Min(value, MaximumValue);
            value = Math.Max(value, MinimumValue);

            if (!float.IsNaN(_lowerMaximumValue))
            {
                value = Math.Min(value, _lowerMaximumValue);
            }

            value = Math.Min(value, UpperValue - MinimumRange);

            LowerValue = value;

            SetNeedsLayout();

            OnLowerValueChanged();
        }

        private void SetUpperValue(float upperValue)
        {
            var value = upperValue;

            if (_stepValueInternal > 0)
            {
                value = RoundToStepValue(value);
            }

            value = Math.Max(value, MinimumValue);
            value = Math.Min(value, MaximumValue);

            if (!float.IsNaN(_upperMinimumValue))
            {
                value = Math.Max(value, _upperMinimumValue);
            }

            value = Math.Max(value, LowerValue + MinimumRange);

            UpperValue = value;

            SetNeedsLayout();

            OnUpperValueChanged();
        }

        private float RoundToStepValue(float value)
        {
            return (float)Math.Round((value - MinimumValue) / _stepValueInternal) * _stepValueInternal + MinimumValue;
        }


        private void SetLowerValue(float lowerValue, float upperValue, bool animated)
        {
            if (!animated && (float.IsNaN(lowerValue) || Math.Abs(lowerValue - LowerValue) < float.Epsilon) &&
                (float.IsNaN(upperValue) || Math.Abs(upperValue - UpperValue) < float.Epsilon))
            {
                //nothing to set
                return;
            }

            Action setValuesBlock = () =>
            {
                if (!float.IsNaN(lowerValue))
                {
                    SetLowerValue(lowerValue);
                }
                if (!float.IsNaN(upperValue))
                {
                    SetUpperValue(upperValue);
                }
            };

            if (animated)
            {
                AnimateNotify(0.25d, 0d, UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
                    setValuesBlock();
                    LayoutSubviews();
                }, finished => { });
            }
            else
            {
                setValuesBlock();
            }
        }

        private void SetLowerValue(float lowerValue, bool animated)
        {
            SetLowerValue(lowerValue, float.NaN, animated);
        }

        private void SetUpperValue(float upperValue, bool animated)
        {
            SetLowerValue(float.NaN, upperValue, animated);
        }

        /// <summary>
        /// ON-Demand images. If the images are not set, then the default values are loaded.
        /// </summary>
        private UIImage ImageFromBundle(string imageName)
        {
            return UIImage.FromFile(imageName);
        }

        /// <summary>
        /// Returns the lower value based on the X potion
        /// The return value is automatically adjust to fit inside the valid range
        /// </summary>
        private float LowerValueForCenterX(float x)
        {
            var padding = (float)_lowerHandle.Frame.Size.Width / 2.0f;
            var value = MinimumValue +
                        (x - padding) / ((float)Frame.Size.Width - padding * 2) * (MaximumValue - MinimumValue);

            value = Math.Max(value, MinimumValue);
            value = Math.Min(value, UpperValue - MinimumRange);

            return value;
        }

        /// <summary>
        /// Returns the upper value based on the X potion
        /// The return value is automatically adjust to fit inside the valid range
        /// </summary>
        private float UpperValueForCenterX(float x)
        {
            var padding = (float)_upperHandle.Frame.Size.Width / 2.0f;

            var value = MinimumValue +
                        (x - padding) / ((float)Frame.Size.Width - padding * 2) * (MaximumValue - MinimumValue);

            value = Math.Min(value, MaximumValue);
            value = Math.Max(value, LowerValue + MinimumRange);

            return value;
        }

        private string ValueToString(float value, Thumb thumb)
        {
            var func = FormatLabel;
            return func == null
                ? value.ToString(_textFormat, CultureInfo.InvariantCulture)
                : func(thumb, value);
        }

        private static CGSize GetTextAboveThumbSize(UILabel label)
        {
            return label.SizeThatFits(new CGSize(float.MaxValue, float.MaxValue));
        }

        private static CGRect Normalise(CGRect retValue) {
            if (nfloat.IsNaN(retValue.Width) || retValue.Width < 0) return new CGRect();
            if (nfloat.IsNaN(retValue.Height) || retValue.Height < 0) return new CGRect();

            return retValue;
        }

        /// <summary>
        /// returns the rect for the track image between the lower and upper values based on the trackimage object
        /// </summary>
        private CGRect TrackRect()
        {
            var retValue = new CGRect();

            retValue.Size = new CGSize(0, BarHeight);

            var lowerHandleWidth = _lowerHandleHidden
                ? _lowerHandleHiddenWidth
                : (float)_lowerHandle.Frame.Size.Width;
            var upperHandleWidth = _upperHandleHidden
                ? _upperHandleHiddenWidth
                : (float)_upperHandle.Frame.Size.Width;

            var xLowerValue = ((float)Bounds.Size.Width - lowerHandleWidth) * (LowerValue - MinimumValue) /
                              (MaximumValue - MinimumValue) + lowerHandleWidth / 2.0f;
            var xUpperValue = ((float)Bounds.Size.Width - upperHandleWidth) * (UpperValue - MinimumValue) /
                              (MaximumValue - MinimumValue) + upperHandleWidth / 2.0f;

            retValue.X = xLowerValue;
            retValue.Y = Bounds.Size.Height / 2.0f - retValue.Size.Height;
            retValue.Width = xUpperValue - xLowerValue;

            var alignmentInsets = TrackAlignmentInsets();
            retValue = alignmentInsets.InsetRect(retValue);

            return Normalise(retValue);
        }

        private CGRect TrackBackgroundRect()
        {
            var rect = new CGRect
            {
                Size = new CGSize(Bounds.Size.Width, BarHeight)
            };

            rect.X = 0;
            rect.Y = Bounds.Size.Height / 2.0f - rect.Size.Height;

            var alignmentInsets = TrackAlignmentInsets();
            rect = alignmentInsets.InsetRect(rect);

            return Normalise(rect);
        }

        private UIEdgeInsets TrackAlignmentInsets()
        {
            var lowerAlignmentInsets = LowerHandleImage.AlignmentRectInsets;
            var upperAlignmentInsets = UpperHandleImage.AlignmentRectInsets;

            var lowerOffset = Math.Max((float)lowerAlignmentInsets.Right, (float)upperAlignmentInsets.Left);
            var upperOffset = Math.Max((float)upperAlignmentInsets.Right, (float)lowerAlignmentInsets.Left);

            var leftOffset = Math.Max(lowerOffset, upperOffset);
            var rightOffset = leftOffset;
            //var topOffset = (float)lowerAlignmentInsets.Top;
            //var bottomOffset = (float)lowerAlignmentInsets.Bottom;

            return new UIEdgeInsets(0, leftOffset, 0, rightOffset);
        }

        /// <summary>
        /// returms the rect of the tumb image for a given track rect and value
        /// </summary>
        private CGRect ThumbRectForValue(float value, UIImage thumbImage)
        {
            var thumbRect = new CGRect();
            var insets = thumbImage.CapInsets;

            thumbRect.Size = new CGSize(thumbImage.Size.Width, thumbImage.Size.Height);

            var height = Bounds.Size.Height - SpaceAboveThumbs;
            if (Math.Abs(insets.Top) > float.Epsilon || Math.Abs(insets.Bottom) > float.Epsilon)
            {
                thumbRect.Height = height;
            }

            var xValue = ((float)Bounds.Size.Width - (float)thumbRect.Size.Width) *
                         ((value - MinimumValue) / (MaximumValue - MinimumValue));
            thumbRect.X = (float)Math.Round(xValue);
            thumbRect.Y = SpaceAboveThumbs + height / 2.0f - thumbRect.Size.Height / 2.0f;

            return Normalise(thumbRect.Integral());
        }

        /// <summary>
        /// returms the rect of the tumb image for a given track rect and value
        /// </summary>
        private CGRect HandleLabelRect(UILabel label, CGRect handleFrame)
        {
            var thumbRect = new CGRect { Size = GetTextAboveThumbSize(label) };

            var xValue = handleFrame.X + handleFrame.Size.Width / 2 - thumbRect.Size.Width / 2;
            thumbRect.X = (float)Math.Round(xValue);
            thumbRect.Y = SpaceAboveThumbs / 2.0f - thumbRect.Size.Height / 2.0f;

            return Normalise(thumbRect.Integral());
        }

        private void AddSubviews()
        {
            Frame = new CGRect(new CGPoint(0, 0), IntrinsicContentSize);
            //------------------------------
            // Lower Handle Handle
            _lowerHandle = new UIImageView(LowerHandleImage)
            {
                Frame = ThumbRectForValue(LowerValue, LowerHandleImage)
            };

            //------------------------------
            // Upper Handle Handle
            _upperHandle = new UIImageView(UpperHandleImage)
            {
                Frame = ThumbRectForValue(UpperValue, UpperHandleImage)
            };

            //------------------------------
            // Track
            _track = new UIView
            {
                Frame = TrackRect(),
                BackgroundColor = UIColor.FromRGB(0, 122, 255)
            };

            //------------------------------
            // Track Brackground
            _trackBackground = new UIView
            {
                Frame = TrackBackgroundRect(),
                BackgroundColor = UIColor.FromRGB(182, 182, 182)
            };

            _lowerHandleLabel = new UILabel
            {
                Text = "123",
                Hidden = !ShowTextAboveThumbs,
                Font = UIFont.SystemFontOfSize(_textSize)
            };

            _upperHandleLabel = new UILabel
            {
                Text = "123",
                Hidden = !ShowTextAboveThumbs,
                Font = UIFont.SystemFontOfSize(_textSize),
                TextColor = _textColor
            };

            if (_textColor != null)
            {
                _lowerHandleLabel.TextColor = _textColor;
                _upperHandleLabel.TextColor = _textColor;
            }

            AddSubview(_trackBackground);
            AddSubview(_track);
            AddSubview(_lowerHandle);
            AddSubview(_upperHandle);
            AddSubview(_lowerHandleLabel);
            AddSubview(_upperHandleLabel);
        }

        public override CGSize SizeThatFits(CGSize size)
        {
            return IntrinsicContentSize;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _trackBackground.Frame = TrackBackgroundRect();
            _track.Frame = TrackRect();

            // Layout the lower handle
            _lowerHandle.Frame = ThumbRectForValue(LowerValue, LowerHandleImage);
            _lowerHandle.Image = LowerHandleImage;
            _lowerHandle.Hidden = _lowerHandleHidden;

            // Layoput the upper handle
            _upperHandle.Frame = ThumbRectForValue(UpperValue, UpperHandleImage);
            _upperHandle.Image = UpperHandleImage;
            _upperHandle.Hidden = _upperHandleHidden;

            _lowerHandleLabel.Text = ValueToString(LowerValue, Thumb.Lower);
            _lowerHandleLabel.Font = UIFont.SystemFontOfSize(_textSize);
            _lowerHandleLabel.Frame = HandleLabelRect(_lowerHandleLabel, _lowerHandle.Frame);
            _lowerHandleLabel.Hidden = !ShowTextAboveThumbs || _lowerHandleHidden;

            _upperHandleLabel.Text = ValueToString(UpperValue, Thumb.Upper);
            _upperHandleLabel.Font = UIFont.SystemFontOfSize(_textSize);
            _upperHandleLabel.Frame = HandleLabelRect(_upperHandleLabel, _upperHandle.Frame);
            _upperHandleLabel.Hidden = !ShowTextAboveThumbs || _upperHandleHidden;

            FixLabelFrames(_trackBackground.Frame, _lowerHandleLabel, _upperHandleLabel);

            if (_textColor != null)
            {
                _lowerHandleLabel.TextColor = _textColor;
                _upperHandleLabel.TextColor = _textColor;
            }
        }

        private void FixLabelFrames(CGRect frame, UILabel lowerHandleLabel, UILabel upperHandleLabel)
        {
            if (lowerHandleLabel.Frame.X < frame.X) {
                lowerHandleLabel.Frame = new CGRect(frame.X, lowerHandleLabel.Frame.Y, lowerHandleLabel.Frame.Width, lowerHandleLabel.Frame.Height);
            }
            if (upperHandleLabel.Frame.Right > frame.Right)
            {
                upperHandleLabel.Frame = new CGRect(frame.Right - upperHandleLabel.Frame.Width, upperHandleLabel.Frame.Y, upperHandleLabel.Frame.Width, upperHandleLabel.Frame.Height);
            }
            var overlap = (lowerHandleLabel.Frame.Right - upperHandleLabel.Frame.X) / 2 + TextLateralPadding;
            if (overlap > 0) {
                lowerHandleLabel.Frame = new CGRect(lowerHandleLabel.Frame.X - overlap, lowerHandleLabel.Frame.Y, lowerHandleLabel.Frame.Width, lowerHandleLabel.Frame.Height);
                upperHandleLabel.Frame = new CGRect(upperHandleLabel.Frame.X + overlap, upperHandleLabel.Frame.Y, upperHandleLabel.Frame.Width, upperHandleLabel.Frame.Height);
            }
        }

        public override bool BeginTracking(UITouch uitouch, UIEvent uievent)
        {
            var touchPoint = uitouch.LocationInView(this);


            //Check both buttons upper and lower thumb handles because
            //they could be on top of each other.

            if (_lowerTouchEdgeInsets.InsetRect(_lowerHandle.Frame).Contains(touchPoint) && !LowerHandleHidden)
            {
                _lowerHandle.Highlighted = true;
                _lowerTouchOffset = (float)touchPoint.X - (float)_lowerHandle.Center.X;
            }

            if (_upperTouchEdgeInsets.InsetRect(_upperHandle.Frame).Contains(touchPoint) && !UpperHandleHidden)
            {
                _upperHandle.Highlighted = true;
                _upperTouchOffset = (float)touchPoint.X - (float)_upperHandle.Center.X;
            }

            _stepValueInternal = StepValueContinuously ? StepValue : 0.0f;

            DragStarted?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public override bool ContinueTracking(UITouch uitouch, UIEvent uievent)
        {
            if (!_lowerHandle.Highlighted && !_upperHandle.Highlighted)
                return true;

            var touchPoint = uitouch.LocationInView(this); //[touch locationInView: self];

            if (_lowerHandle.Highlighted)
            {
                //get new lower value based on the touch location.
                //This is automatically contained within a valid range.
                var newValue = LowerValueForCenterX((float)touchPoint.X - _lowerTouchOffset);

                //if both upper and lower is selected, then the new value must be LOWER
                //otherwise the touch event is ignored.
                if (!_upperHandle.Highlighted || newValue < LowerValue)
                {
                    _upperHandle.Highlighted = false;
                    BringSubviewToFront(_lowerHandle);
                    SetLowerValue(newValue, StepValueContinuously);
                }
                else
                {
                    _lowerHandle.Highlighted = false;
                }
            }

            if (_upperHandle.Highlighted)
            {
                var newValue = UpperValueForCenterX((float)touchPoint.X - _upperTouchOffset);

                //if both upper and lower is selected, then the new value must be HIGHER
                //otherwise the touch event is ignored.
                if (!_lowerHandle.Highlighted || newValue > UpperValue)
                {
                    _lowerHandle.Highlighted = false;
                    BringSubviewToFront(_upperHandle);
                    SetUpperValue(newValue, StepValueContinuously);
                }
                else
                {
                    _upperHandle.Highlighted = false;
                }
            }


            //send the control event
            if (Continuous)
                SendActionForControlEvents(UIControlEvent.ValueChanged);

            //redraw
            SetNeedsLayout();

            return true;
        }

        public override void EndTracking(UITouch uitouch, UIEvent uievent)
        {
            _lowerHandle.Highlighted = false;
            _upperHandle.Highlighted = false;

            if (StepValue > 0)
            {
                _stepValueInternal = StepValue;

                SetLowerValue(LowerValue, true);
                SetUpperValue(UpperValue, true);
            }

            SendActionForControlEvents(UIControlEvent.ValueChanged);

            DragCompleted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnLowerValueChanged()
        {
            LowerValueChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnUpperValueChanged()
        {
            UpperValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}