using ASSETPackANIMATIONS.Lookups;

namespace ASSETPackANIMATIONS.Actions
{
    public class Dodge : InstantActionHandler<DodgeType>
    {
        public override bool CanStartAction(RPGCharacterController controller)
        { return controller.canAction && !controller.IsActive("Relax"); }

        protected override void _StartAction(RPGCharacterController controller, DodgeType dodgeType)
        {
            controller.GetAngry();
            controller.Dodge(dodgeType);
        }
    }
}