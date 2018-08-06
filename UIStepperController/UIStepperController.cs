using System;
using CoreGraphics;
using Foundation;
using UIKit;
// ReSharper disable UnusedMember.Global

namespace UIStepController
{
    [Register("UIStepperController")]
    public class UIStepperController : UIView
    {
        #region field 

        private NSObject _textValueChange;
        private readonly UITextField _lblCounter = new UITextField { KeyboardType = UIKeyboardType.NumbersAndPunctuation };
        private readonly UIView _additionButtonView = new UIView();
        private readonly UIView _subtractionButtonView = new UIView();
        private readonly UIImageView _additionButtonImage = new UIImageView();
        private readonly UIImageView _subtractionButtonImage = new UIImageView();
        private readonly UIView _crossShape01 = new UIView();
        private readonly UIView _crossShape02 = new UIView();
        private readonly UIView _crossShape03 = new UIView();
        private readonly UIView _additionIconView = new UIView();
        private readonly UIView _subtractionIconView = new UIView();

        public event EventHandler<float> OnValueChanged;

        #endregion

        #region Propertys
        
        public bool IsFloat { get; set; }

        public float Increment { get; set; } = 1;

        public float? MinValue { get; set; } 
        public float? MaxValue { get; set; }

        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                    return;
                _value = value;
                UpdateCountTextField();

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

        public UIColor TextColor
        {
            get => _lblCounter.TextColor;
            set => _lblCounter.TextColor = value;
        }

        #endregion

        #region Methodes
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
    
        public void BorderColor(UIColor color)
        {
            Layer.BorderColor = color.CGColor;
            _additionButtonView.Layer.BorderColor = color.CGColor;
            _subtractionButtonView.Layer.BorderColor = color.CGColor;
            _crossShape01.BackgroundColor = color;
            _crossShape02.BackgroundColor = color;
            _crossShape03.BackgroundColor = color;
        }
        #endregion

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

        protected internal UIStepperController(IntPtr handle)
            : base(handle)
        {
            CommonInit();
        }

        #endregion

        #region Build of view

        private void CommonInit()
        {

            // ************** Update TextField *********
            _textValueChange = NSNotificationCenter.DefaultCenter.AddObserver(UITextField.TextFieldTextDidChangeNotification, TextChangedEvent);

            // Self view customize
            Layer.BorderColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1.0f).CGColor;
            Layer.BorderWidth = 1;
            Layer.CornerRadius = 5;
            Layer.MasksToBounds = true;
            BackgroundColor = UIColor.White;

            // *************** Center UILabel ****************
            _lblCounter.Font = UIFont.SystemFontOfSize(Frame.Size.Height / 2);
            _lblCounter.TextColor = UIColor.FromRGBA(1f, 0.68f, 0f, 1f);
            UpdateCountTextField();
            _lblCounter.TextAlignment = UITextAlignment.Center;
            _lblCounter.BackgroundColor = UIColor.Clear;

            //Adding constraints for _lblCounter
            _lblCounter.TranslatesAutoresizingMaskIntoConstraints = false;
            AddConstraints(new[]
            {
                NSLayoutConstraint.Create(_lblCounter,NSLayoutAttribute.Top,NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(_lblCounter,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,this,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(_lblCounter,NSLayoutAttribute.Right,NSLayoutRelation.Equal,_additionButtonView,NSLayoutAttribute.Right,1,0),
                NSLayoutConstraint.Create(_lblCounter,NSLayoutAttribute.Left,NSLayoutRelation.Equal,_subtractionButtonView,NSLayoutAttribute.Left,1,0)
            });

            AddSubview(_lblCounter);

            // *************** Left Button ****************
            _subtractionButtonView.Layer.BorderColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1f).CGColor;
            _subtractionButtonView.Layer.BorderWidth = 1;
            _subtractionButtonView.BackgroundColor = UIColor.Clear;
           
            // Add constraints to Left button
            _subtractionButtonView.TranslatesAutoresizingMaskIntoConstraints = false;
            AddConstraints(new[]
            {
                NSLayoutConstraint.Create(_subtractionButtonView,NSLayoutAttribute.Width,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,32),
                NSLayoutConstraint.Create(_subtractionButtonView,NSLayoutAttribute.Top,NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(_subtractionButtonView,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,this,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(_subtractionButtonView,NSLayoutAttribute.Left,NSLayoutRelation.Equal,this,NSLayoutAttribute.Left,1,0)
            });


            // Subtraction button's image
            _subtractionButtonImage.TranslatesAutoresizingMaskIntoConstraints = false;
            _subtractionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(_subtractionButtonView,NSLayoutAttribute.Top,NSLayoutRelation.Equal,_subtractionButtonView,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(_subtractionButtonView,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,_subtractionButtonView,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(_subtractionButtonView,NSLayoutAttribute.Right,NSLayoutRelation.Equal,_subtractionButtonView,NSLayoutAttribute.Right,1,0),
                NSLayoutConstraint.Create(_subtractionButtonView,NSLayoutAttribute.Left,NSLayoutRelation.Equal,_subtractionButtonView,NSLayoutAttribute.Left,1,0)
            });
            
            _subtractionButtonImage.Hidden = false;
            _subtractionButtonView.AddSubview(_subtractionButtonImage);


            // Subtraction button's icon view
            _subtractionIconView.TranslatesAutoresizingMaskIntoConstraints = false;
            _subtractionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(_subtractionIconView,NSLayoutAttribute.Height,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,13),
                NSLayoutConstraint.Create(_subtractionIconView,NSLayoutAttribute.Width,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,13),
                NSLayoutConstraint.Create(_subtractionIconView,NSLayoutAttribute.CenterX,NSLayoutRelation.Equal,_subtractionButtonView,NSLayoutAttribute.CenterX,1,0),
                NSLayoutConstraint.Create(_subtractionIconView,NSLayoutAttribute.CenterY,NSLayoutRelation.Equal,_subtractionButtonView,NSLayoutAttribute.CenterY,1,0)

            });

            _subtractionButtonView.AddSubview(_subtractionIconView);

            // Subtraction button's icon shape
            _crossShape01.Frame = new CGRect(0, 5.5, 13, 2);
            _crossShape01.BackgroundColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1f);

            _subtractionIconView.AddSubview(_crossShape01);

            // Subtraction button's Touch Event
            var subtractionButton = new UIButton { TranslatesAutoresizingMaskIntoConstraints = false };
            subtractionButton.TouchUpInside += SubtractionButtonOnTouchUpInside;
            _subtractionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(subtractionButton,NSLayoutAttribute.Top,NSLayoutRelation.Equal,_subtractionButtonView,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(subtractionButton,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,_subtractionButtonView,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(subtractionButton,NSLayoutAttribute.Right,NSLayoutRelation.Equal,_subtractionButtonView,NSLayoutAttribute.Right,1,0),
                NSLayoutConstraint.Create(subtractionButton,NSLayoutAttribute.Left,NSLayoutRelation.Equal,_subtractionButtonView,NSLayoutAttribute.Left,1,0)
            });

            _subtractionButtonView.AddSubview(subtractionButton);
            AddSubview(_subtractionButtonView);


            // *************** Right Button ****************
            _additionButtonView.Layer.BorderColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1f).CGColor;
            _additionButtonView.Layer.BorderWidth = 1;
            _additionButtonView.BackgroundColor = UIColor.Clear;

            //Adding constraints to right button
            _additionButtonView.TranslatesAutoresizingMaskIntoConstraints = false;
            AddConstraints(new[]
            {
                NSLayoutConstraint.Create(_additionButtonView,NSLayoutAttribute.Width,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,32),
                NSLayoutConstraint.Create(_additionButtonView,NSLayoutAttribute.Top,NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(_additionButtonView,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,this,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(_additionButtonView,NSLayoutAttribute.Right,NSLayoutRelation.Equal,this,NSLayoutAttribute.Right,1,0)
            });

            // Addition button's image
            _additionButtonImage.TranslatesAutoresizingMaskIntoConstraints = false;
            _additionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(_additionButtonImage,NSLayoutAttribute.Top,NSLayoutRelation.Equal,_additionButtonView,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(_additionButtonImage,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,_additionButtonView,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(_additionButtonImage,NSLayoutAttribute.Right,NSLayoutRelation.Equal,_additionButtonView,NSLayoutAttribute.Right,1,0),
                NSLayoutConstraint.Create(_additionButtonImage,NSLayoutAttribute.Left,NSLayoutRelation.Equal,_additionButtonView,NSLayoutAttribute.Left,1,0)
            });

            _additionButtonImage.Hidden = true;
            _additionButtonView.AddSubview(_additionButtonImage);

            // Addition button's icon view
            _additionIconView.TranslatesAutoresizingMaskIntoConstraints = false;
            _additionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(_additionIconView,NSLayoutAttribute.Height,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,13),
                NSLayoutConstraint.Create(_additionIconView,NSLayoutAttribute.Width,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,13),
                NSLayoutConstraint.Create(_additionIconView,NSLayoutAttribute.CenterX,NSLayoutRelation.Equal,_additionButtonView,NSLayoutAttribute.CenterX,1,0),
                NSLayoutConstraint.Create(_additionIconView,NSLayoutAttribute.CenterY,NSLayoutRelation.Equal,_additionButtonView,NSLayoutAttribute.CenterY,1,0)
            });

            _additionButtonView.AddSubview(_additionIconView);

            // Addition button's icon shapes
            _crossShape02.Frame = new CGRect(5.5, 0, 2, 13);
            _crossShape02.BackgroundColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1f);
            _additionIconView.AddSubview(_crossShape02);

            _crossShape03.Frame = new CGRect(0, 5.5, 13, 2);
            _crossShape03.BackgroundColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1f);
            _additionIconView.AddSubview(_crossShape03);

            // Addition button's Touch Even
            var additionButton = new UIButton { TranslatesAutoresizingMaskIntoConstraints = false };
            additionButton.TouchUpInside += AdditionButton_TouchUpInside;

            _additionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(additionButton,NSLayoutAttribute.Top,NSLayoutRelation.Equal,_additionButtonView,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(additionButton,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,_additionButtonView,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(additionButton,NSLayoutAttribute.Right,NSLayoutRelation.Equal,_additionButtonView,NSLayoutAttribute.Right,1,0),
                NSLayoutConstraint.Create(additionButton,NSLayoutAttribute.Left,NSLayoutRelation.Equal,_additionButtonView,NSLayoutAttribute.Left,1,0)
            });

            _additionButtonView.AddSubview(additionButton);
            AddSubview(_additionButtonView);

        }

        private void TextChangedEvent(NSNotification obj)
        {
            
            if (!(obj.Object is UITextField field) || !Equals(field,_lblCounter)) return;
            if (float.TryParse(field.Text, out var fl))
                Value = fl;
            else
                UpdateCountTextField();
            
        }

        #endregion

        private void UpdateCountTextField()
        {
            IsFloat = !IsInt(Value);
            _lblCounter.Text = IsFloat ? $"{Value:F}" : $"{Value:N0}";
        }

        #region Button Click

        private void SubtractionButtonOnTouchUpInside(object sender, EventArgs e)
        {
            
            var c = Value - Increment;

            if(MinValue.HasValue && c < MinValue.Value)return;

            Value = c;
            OnValueChanged?.Invoke(this, Value);
       
        }
        private void AdditionButton_TouchUpInside(object sender, EventArgs e)
        {
            var c = Value + Increment;

            if (MaxValue.HasValue && MaxValue.Value > c)return;
            Value = c;
                
            OnValueChanged?.Invoke(this, Value);
        }

        protected override void Dispose(bool disposing)
        {
            _textValueChange?.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}