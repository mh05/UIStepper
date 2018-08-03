using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace UIStepController
{
    public class UIStepperController : UIView
    {
        #region field 


        private readonly UITextField _lblCounter = new UITextField { KeyboardType = UIKeyboardType.NumbersAndPunctuation };
        private readonly UIView _additionButtonView = new UIView();
        private readonly UIView _subtractionButtonView = new UIView();
        private readonly UIImageView _additionButtonImage = new UIImageView();
        private readonly UIImageView _subtractionButtonImage = new UIImageView();
        private readonly UIView _crossShape01 = new UIView();
        private readonly UIView _crossShape02 = new UIView();
        private readonly UIView _crossShape03 = new UIView();


        #endregion

        public IUIStepControllerDelegate Delegate { get; set; }
        public bool IsMinus { get; set; }
        public bool IsFloat { get; set; }

        public float Incrementer { get; set; } = 1;

        private float _count = 0f;
        public float Count
        {
            get => _count;
            set
            {
                if (Equals(_count, value))
                    return;
                _count = value;
                IsFloat = !IsInt(value);
                _lblCounter.Text = IsFloat ? $"{Count:F}" : $"{Count:N0}";

            }
        }

        private static bool IsInt(float f) => Math.Abs(f % 1) <= float.Epsilon * 100;


        public UIColor LeftButtonBackgroundColor
        {
            get => _subtractionButtonView.BackgroundColor;
            set => _subtractionButtonView.BackgroundColor = value;
        }

        public UIColor LeftButtonForegroundColor
        {
            get => _crossShape01.BackgroundColor;
            set => _crossShape01.BackgroundColor = value;
        }

        public UIColor RightButtonBackgroundColor
        {
            get => _additionButtonView.BackgroundColor;
            set => _additionButtonView.BackgroundColor = value;
        }

        public UIColor RightButtonForegroundColor
        {
            get => _crossShape02.BackgroundColor;
            set
            {
                _crossShape02.BackgroundColor = value;
                _crossShape03.BackgroundColor = value;
            }
        }

        public void SetImageToLeftButton(UIImage image, UIViewContentMode contentMode)
        {
            if (image == null)
            {
                _crossShape01.Hidden = false;
                if (_subtractionButtonImage.IsDescendantOfView(_subtractionButtonView))
                    _subtractionButtonImage.RemoveFromSuperview();

            }
            else
            {
                _crossShape01.Hidden = true;
                _subtractionButtonImage.Frame = new CGRect(0, 0, _subtractionButtonView.Frame.Width, height: _subtractionButtonView.Frame.Height);
                _subtractionButtonImage.Image = image;
                _subtractionButtonImage.ContentMode = contentMode;
                _subtractionButtonView.AddSubview(_subtractionButtonImage);
            }
        }


        public void SetImageToRightButton(UIImage image, UIViewContentMode contentMode)
        {
            if (image == null)
            {
                _crossShape02.Hidden = false;
                _crossShape03.Hidden = false;
                if (_additionButtonImage.IsDescendantOfView(_additionButtonView))
                    _additionButtonImage.RemoveFromSuperview();
            }
            else
            {
                _crossShape02.Hidden = true;
                _crossShape03.Hidden = true;
                _additionButtonImage.Frame = new CGRect(0, 0, _additionButtonView.Frame.Width, _additionButtonView.Frame.Height);
                _additionButtonImage.Image = image;
                _additionButtonImage.ContentMode = contentMode;
                _additionButtonView.AddSubview(_additionButtonImage);
            }
        }


        public UIColor TextColor
        {
            get => _lblCounter.TextColor;
            set => _lblCounter.TextColor = value;
        }

        public void BorderColor(UIColor color)
        {
            Layer.BorderColor = color.CGColor;
            _additionButtonView.Layer.BorderColor = color.CGColor;
            _subtractionButtonView.Layer.BorderColor = color.CGColor;
            _crossShape01.BackgroundColor = color;
            _crossShape02.BackgroundColor = color;
            _crossShape03.BackgroundColor = color;
        }


        #region Ctor

        //We need to have an frame so we can create buttons
        public UIStepperController() : this(CGRect.Empty) { }
        public UIStepperController(CGRect frame)
            : base(frame)
        {
            CommonInit();
        }

        public UIStepperController(NSCoder coder)
            : base(coder)
        {
            CommonInit();
        }

        #endregion

        #region Build of view

        private void CommonInit()
        {
            // *************** Left Button ****************
            _subtractionButtonView.Frame = new CGRect(0, 0, 32, Frame.Size.Height);
            _subtractionButtonView.Layer.BorderColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1.0f).CGColor;
            _subtractionButtonView.Layer.BorderWidth = 1f;
            _subtractionButtonView.BackgroundColor = UIColor.Clear;

            _crossShape01.Frame = new CGRect(_subtractionButtonView.Frame.Width / 2 - 6.5, _subtractionButtonView.Frame.Height / 2 - 1, 13, 2);
            _crossShape01.BackgroundColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1.0f);
            _subtractionButtonView.AddSubview(_crossShape01);

            var subtractionButton = new UIButton(new CGRect(0, 0, 32, Frame.Size.Height));
            subtractionButton.TouchUpInside += SubtractionButtonOnTouchUpInside;
            _subtractionButtonView.AddSubview(subtractionButton);

            AddSubview(_subtractionButtonView);

            // *************** Right Button ****************
            _additionButtonView.Frame = new CGRect(Frame.Size.Width - 32, 0, 32, Frame.Size.Height);
            _additionButtonView.Layer.BorderColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1.0f).CGColor;
            _additionButtonView.Layer.BorderWidth = 1f;
            _additionButtonView.BackgroundColor = UIColor.Clear;


            _crossShape02.Frame = new CGRect(_additionButtonView.Frame.Width / 2 - 1, _additionButtonView.Frame.Height / 2 - 6.5, 2, 13);
            _crossShape02.BackgroundColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1.0f);
            _additionButtonView.AddSubview(_crossShape02);

            _crossShape03.Frame = new CGRect(_additionButtonView.Frame.Width / 2 - 6.5,
                _additionButtonView.Frame.Height / 2 - 1, 13, 2);

            _crossShape03.BackgroundColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1.0f);
            _additionButtonView.AddSubview(_crossShape03);

            var additionButton = new UIButton(new CGRect(0, 0, 32, Frame.Size.Height));
            additionButton.TouchUpInside += AdditionButton_TouchUpInside;
            _additionButtonView.AddSubview(additionButton);
            AddSubview(_additionButtonView);

            _lblCounter.Frame = new CGRect(32, 0, Frame.Size.Width - 64, Frame.Size.Height);
            _lblCounter.Font = UIFont.SystemFontOfSize(Frame.Size.Height / 2);
            _lblCounter.TextColor = UIColor.FromRGBA(1.0f, 0.68f, 0.0f, 1.0f);
            _lblCounter.Text = "0";
            // Count
            _lblCounter.TextAlignment = UITextAlignment.Center;
            _lblCounter.BackgroundColor = UIColor.Clear;
            AddSubview(_lblCounter);

            //View customize
            Frame = new CGRect(Frame.X, Frame.Y, Frame.Size.Width, Frame.Size.Height);
            Layer.BorderColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1.0f).CGColor;
            Layer.BorderWidth = 1f;
            Layer.CornerRadius = 5;
            Layer.MasksToBounds = true;
            BackgroundColor = UIColor.White;

        }

        #endregion


        #region Button Click

        private void SubtractionButtonOnTouchUpInside(object sender, EventArgs e)
        {
            var c = Count - Incrementer;
            Count = c > 0 ? c : (IsMinus ? c : 0);
            Delegate?.stepperDidSubtractValues(this);
        }
        private void AdditionButton_TouchUpInside(object sender, EventArgs e)
        {
            Count += Incrementer;
            Delegate?.stepperDidAddValues(this);
        }

        #endregion
    }
}