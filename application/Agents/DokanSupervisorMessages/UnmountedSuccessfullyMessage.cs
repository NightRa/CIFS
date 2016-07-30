namespace Agents.DokanSupervisorMessages
{
    public sealed class UnmountedSuccessfullyMessage : DokanSupervisorMessage
    {
        public override string AsString()
        {
            return "Dokan unmounted successfully";
        }
    }
}
