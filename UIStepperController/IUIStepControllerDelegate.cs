namespace UIStepController
{
    public interface IUIStepControllerDelegate
    {
        void stepperDidAddValues(UIStepperController stepper);
        void stepperDidSubtractValues(UIStepperController stepper);
    }
}