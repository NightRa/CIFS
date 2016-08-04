namespace Agents.AdministratorMessages
{
    public sealed class DokanExitedSuccessfullyMessage : AdministratorMessage
    {
        public override string AsString()
        {
            return "Dokan exited successfully";
        }
    }
}
