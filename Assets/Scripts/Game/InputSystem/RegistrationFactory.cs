namespace Game.InputSystem
{
    public static class RegistrationFactory
    {
        public static Registration Create(string inputName, IDirectionCommand command)
        {
            Registration registration = new JoystickRegistration(inputName, command);
            return registration;
        }

        public static Registration Create(string inputName, IContinuousCommand command)
        {
            Registration registration = new ButtonRegistration(inputName, command);
            return registration;
        }

        public static Registration Create(string inputName, IImpulseCommand command)
        {
            Registration registration = new ButtonDownRegistration(inputName, command);
            return  registration;
        }
    }
}
