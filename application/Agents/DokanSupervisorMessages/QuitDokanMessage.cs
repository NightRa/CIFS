namespace Agents.DokanSupervisorMessages
{
    public sealed class QuitDokanRequestMessage : DokanSupervisorMessage   
    {
        public override string AsString()
        {
            return "Quit Dokan is requested";
        }
    }
}
