namespace Agents.AdministratorMessages
{
    public sealed class QuitAppMessage : AdministratorMessage
    {
        public override string AsString()
        {
            return "Quit app request";
        }
    }
}
