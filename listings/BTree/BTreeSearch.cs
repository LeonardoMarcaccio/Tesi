public EntryPage<TK, TP>? Search(TK key)
{
    return this.SearchInternal(this._rootPage, key);
}

private EntryPage<TK, TP>? SearchInternal(NodePage<TK, TP> node, TK key)
{
    int i = node.Entries.TakeWhile(entry => key.CompareTo(entry.Key) > 0).Count();
    if (i < node.Entries.Count && node.Entries[i].Key.CompareTo(key) == 0)
    {
        return node.Entries[i]._entryPage;
    }
    if (node.IsLeaf)
    {
        return null;
    }
    else
    {
        EntryPage<TK, TP>? entryPage;
        foreach (var item in node.Children)
        {
            entryPage = this.SearchInternal(node.Children[i]._nodePage, key);
            if (entryPage != null)
            {
                return entryPage;
            }
        }
        return null;
    }
}
