namespace Agents.DokanMessages
{
    public sealed class UnmountedSuccessfullyMessage : DokanMessage
    {
        public override string AsString()
        {
            return "Dokan unmounted successfully";
        }
    }
}
