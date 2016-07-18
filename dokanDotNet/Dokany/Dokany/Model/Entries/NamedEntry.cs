namespace Dokany.Model.Entries
{
    public sealed class NamedEntry
    {
        public Entry Entry { get; }
        public string Name { get; }

        public NamedEntry(Entry entry, string name)
        {
            this.Entry = entry;
            this.Name = name;
        }
    }
}
