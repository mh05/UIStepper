using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace UIStepController
{
    [Register("UIStepperController")]
    public class UIStepperController : UIView
    {
        #region field 

        private NSObject _textValueChange;
        private readonly UITextField countLable = new UITextField { KeyboardType = UIKeyboardType.NumbersAndPunctuation };
        private readonly UIView additionButtonView = new UIView();
        private readonly UIView subtractionButtonView = new UIView();
        private readonly UIImageView additionButtonImage = new UIImageView();
        private readonly UIImageView subtractionButtonImage = new UIImageView();
        private readonly UIView crossShape01 = new UIView();
        private readonly UIView crossShape02 = new UIView();
        private readonly UIView crossShape03 = new UIView();
        private readonly UIView additionIconView = new UIView();
        private readonly UIView subtractionIconView = new UIView();

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
            get => subtractionButtonView.BackgroundColor;
            set => subtractionButtonView.BackgroundColor = value;
        }

        public UIColor LeftButtonForegroundColor
        {
            get => crossShape01.BackgroundColor;
            set => crossShape01.BackgroundColor = value;
        }

        public UIColor RightButtonBackgroundColor
        {
            get => additionButtonView.BackgroundColor;
            set => additionButtonView.BackgroundColor = value;
        }

        public UIColor RightButtonForegroundColor
        {
            get => crossShape02.BackgroundColor;
            set
            {
                crossShape02.BackgroundColor = value;
                crossShape03.BackgroundColor = value;
            }
        }

        public UIColor TextColor
        {
            get => countLable.TextColor;
            set => countLable.TextColor = value;
        }

        #endregion

        #region Methodes
        public void SetImageToLeftButton(UIImage image, UIViewContentMode contentMode)
        {
            if (image == null)
            {
                crossShape01.Hidden = false;
                if (subtractionButtonImage.IsDescendantOfView(subtractionButtonView))
                    subtractionButtonImage.RemoveFromSuperview();

            }
            else
            {
                crossShape01.Hidden = true;
                subtractionButtonImage.Frame = new CGRect(0, 0, subtractionButtonView.Frame.Width, height: subtractionButtonView.Frame.Height);
                subtractionButtonImage.Image = image;
                subtractionButtonImage.ContentMode = contentMode;
                subtractionButtonView.AddSubview(subtractionButtonImage);
            }
        }


        public void SetImageToRightButton(UIImage image, UIViewContentMode contentMode)
        {
            if (image == null)
            {
                crossShape02.Hidden = false;
                crossShape03.Hidden = false;
                if (additionButtonImage.IsDescendantOfView(additionButtonView))
                    additionButtonImage.RemoveFromSuperview();
            }
            else
            {
                crossShape02.Hidden = true;
                crossShape03.Hidden = true;
                additionButtonImage.Frame = new CGRect(0, 0, additionButtonView.Frame.Width, additionButtonView.Frame.Height);
                additionButtonImage.Image = image;
                additionButtonImage.ContentMode = contentMode;
                additionButtonView.AddSubview(additionButtonImage);
            }
        }
    
        public void BorderColor(UIColor color)
        {
            Layer.BorderColor = color.CGColor;
            additionButtonView.Layer.BorderColor = color.CGColor;
            subtractionButtonView.Layer.BorderColor = color.CGColor;
            crossShape01.BackgroundColor = color;
            crossShape02.BackgroundColor = color;
            crossShape03.BackgroundColor = color;
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

            countLable.Font = UIFont.SystemFontOfSize(Frame.Size.Height / 2);
            countLable.TextColor = UIColor.FromRGBA(1f, 0.68f, 0f, 1f);
            UpdateCountTextField();
            countLable.TextAlignment = UITextAlignment.Center;
            countLable.BackgroundColor = UIColor.Clear;

            //Adding constraints to countlable
            countLable.TranslatesAutoresizingMaskIntoConstraints = false;
            AddConstraints(new[]
            {
                NSLayoutConstraint.Create(countLable,NSLayoutAttribute.Top,NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(countLable,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,this,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(countLable,NSLayoutAttribute.Right,NSLayoutRelation.Equal,additionButtonView,NSLayoutAttribute.Right,1,0),
                NSLayoutConstraint.Create(countLable,NSLayoutAttribute.Left,NSLayoutRelation.Equal,subtractionButtonView,NSLayoutAttribute.Left,1,0)
            });

            AddSubview(countLable);

            // *************** Left Button ****************

            subtractionButtonView.Layer.BorderColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1f).CGColor;
            subtractionButtonView.Layer.BorderWidth = 1;
            subtractionButtonView.BackgroundColor = UIColor.Clear;
           

            // Add constraints to Left button
            subtractionButtonView.TranslatesAutoresizingMaskIntoConstraints = false;
            AddConstraints(new[]
            {
                NSLayoutConstraint.Create(subtractionButtonView,NSLayoutAttribute.Width,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,32),
                NSLayoutConstraint.Create(subtractionButtonView,NSLayoutAttribute.Top,NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(subtractionButtonView,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,this,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(subtractionButtonView,NSLayoutAttribute.Left,NSLayoutRelation.Equal,this,NSLayoutAttribute.Left,1,0)
            });


            // Subtraction button's image
            subtractionButtonImage.TranslatesAutoresizingMaskIntoConstraints = false;
            subtractionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(subtractionButtonView,NSLayoutAttribute.Top,NSLayoutRelation.Equal,subtractionButtonView,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(subtractionButtonView,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,subtractionButtonView,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(subtractionButtonView,NSLayoutAttribute.Right,NSLayoutRelation.Equal,subtractionButtonView,NSLayoutAttribute.Right,1,0),
                NSLayoutConstraint.Create(subtractionButtonView,NSLayoutAttribute.Left,NSLayoutRelation.Equal,subtractionButtonView,NSLayoutAttribute.Left,1,0)
            });


            subtractionButtonImage.Hidden = false;
            subtractionButtonView.AddSubview(subtractionButtonImage);


            // Subtraction button's icon view
            subtractionIconView.TranslatesAutoresizingMaskIntoConstraints = false;
            subtractionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(subtractionIconView,NSLayoutAttribute.Height,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,13),
                NSLayoutConstraint.Create(subtractionIconView,NSLayoutAttribute.Width,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,13),
                NSLayoutConstraint.Create(subtractionIconView,NSLayoutAttribute.CenterX,NSLayoutRelation.Equal,subtractionButtonView,NSLayoutAttribute.CenterX,1,0),
                NSLayoutConstraint.Create(subtractionIconView,NSLayoutAttribute.CenterY,NSLayoutRelation.Equal,subtractionButtonView,NSLayoutAttribute.CenterY,1,0)

            });

            subtractionButtonView.AddSubview(subtractionIconView);

            // Subtraction button's icon shape
            crossShape01.Frame = new CGRect(0, 5.5, 13, 2);
            crossShape01.BackgroundColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1f);

            subtractionIconView.AddSubview(crossShape01);

            // Subtraction button's Touch Event
            var subtractionButton = new UIButton { TranslatesAutoresizingMaskIntoConstraints = false };
            subtractionButton.TouchUpInside += SubtractionButtonOnTouchUpInside;
            subtractionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(subtractionButton,NSLayoutAttribute.Top,NSLayoutRelation.Equal,subtractionButtonView,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(subtractionButton,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,subtractionButtonView,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(subtractionButton,NSLayoutAttribute.Right,NSLayoutRelation.Equal,subtractionButtonView,NSLayoutAttribute.Right,1,0),
                NSLayoutConstraint.Create(subtractionButton,NSLayoutAttribute.Left,NSLayoutRelation.Equal,subtractionButtonView,NSLayoutAttribute.Left,1,0)
            });

            subtractionButtonView.AddSubview(subtractionButton);
            AddSubview(subtractionButtonView);


            // *************** Right Button ****************
            additionButtonView.Layer.BorderColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1f).CGColor;
            additionButtonView.Layer.BorderWidth = 1;
            additionButtonView.BackgroundColor = UIColor.Clear;

            //Adding constraints to right button
            additionButtonView.TranslatesAutoresizingMaskIntoConstraints = false;
            AddConstraints(new[]
            {
                NSLayoutConstraint.Create(additionButtonView,NSLayoutAttribute.Width,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,32),
                NSLayoutConstraint.Create(additionButtonView,NSLayoutAttribute.Top,NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(additionButtonView,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,this,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(additionButtonView,NSLayoutAttribute.Right,NSLayoutRelation.Equal,this,NSLayoutAttribute.Right,1,0)
            });

            // Addition button's image
            additionButtonImage.TranslatesAutoresizingMaskIntoConstraints = false;
            additionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(additionButtonImage,NSLayoutAttribute.Top,NSLayoutRelation.Equal,additionButtonView,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(additionButtonImage,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,additionButtonView,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(additionButtonImage,NSLayoutAttribute.Right,NSLayoutRelation.Equal,additionButtonView,NSLayoutAttribute.Right,1,0),
                NSLayoutConstraint.Create(additionButtonImage,NSLayoutAttribute.Left,NSLayoutRelation.Equal,additionButtonView,NSLayoutAttribute.Left,1,0)
            });

            additionButtonImage.Hidden = true;
            additionButtonView.AddSubview(additionButtonImage);

            // Addition button's icon view
            additionIconView.TranslatesAutoresizingMaskIntoConstraints = false;
            additionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(additionIconView,NSLayoutAttribute.Height,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,13),
                NSLayoutConstraint.Create(additionIconView,NSLayoutAttribute.Width,NSLayoutRelation.Equal,null,NSLayoutAttribute.NoAttribute,1,13),
                NSLayoutConstraint.Create(additionIconView,NSLayoutAttribute.CenterX,NSLayoutRelation.Equal,additionButtonView,NSLayoutAttribute.CenterX,1,0),
                NSLayoutConstraint.Create(additionIconView,NSLayoutAttribute.CenterY,NSLayoutRelation.Equal,additionButtonView,NSLayoutAttribute.CenterY,1,0)
            });

            additionButtonView.AddSubview(additionIconView);

            // Addition button's icon shapes
            crossShape02.Frame = new CGRect(5.5, 0, 2, 13);
            crossShape02.BackgroundColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1f);
            additionIconView.AddSubview(crossShape02);

            crossShape03.Frame = new CGRect(0, 5.5, 13, 2);
            crossShape03.BackgroundColor = UIColor.FromRGBA(0.1f, 0.54f, 0.84f, 1f);
            additionIconView.AddSubview(crossShape03);

            // Addition button's Touch Even
            var additionButton = new UIButton { TranslatesAutoresizingMaskIntoConstraints = false };
            additionButton.TouchUpInside += AdditionButton_TouchUpInside;

            additionButtonView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(additionButton,NSLayoutAttribute.Top,NSLayoutRelation.Equal,additionButtonView,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(additionButton,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,additionButtonView,NSLayoutAttribute.Bottom,1,0),
                NSLayoutConstraint.Create(additionButton,NSLayoutAttribute.Right,NSLayoutRelation.Equal,additionButtonView,NSLayoutAttribute.Right,1,0),
                NSLayoutConstraint.Create(additionButton,NSLayoutAttribute.Left,NSLayoutRelation.Equal,additionButtonView,NSLayoutAttribute.Left,1,0)
            });

            additionButtonView.AddSubview(additionButton);
            AddSubview(additionButtonView);

        }

        private void TextChangedEvent(NSNotification obj)
        {
            
            if (!(obj.Object is UITextField field) || !Equals(field,countLable)) return;
            if (float.TryParse(field.Text, out var fl))
                Value = fl;
            else
                UpdateCountTextField();
            
        }

        #endregion

        private void UpdateCountTextField()
        {
            IsFloat = !IsInt(Value);
            countLable.Text = IsFloat ? $"{Value:F}" : $"{Value:N0}";
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


        #endregion
    }
}