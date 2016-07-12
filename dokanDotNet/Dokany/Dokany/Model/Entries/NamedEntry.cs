﻿namespace Dokany.Model.Entries
{
    public sealed class NamedEntry
    {
        public readonly Entry entry;
        public readonly string name;

        public NamedEntry(Entry entry, string name)
        {
            this.entry = entry;
            this.name = name;
        }
    }
}
