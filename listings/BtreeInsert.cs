public void Insert(TK newKey, IEnumerable<TP> data)
{
    if (!this._rootPage.HasReachedMaxEntries)
    {
        InsertNonFull(this._rootPage, newKey, data);
        UpdateLog(this._rootPage);
        WriteAllBuffersContents();
        Serialize();
        return;
    }
    NodePage<TK, TP> oldRoot = this._rootPage;
    CreateNewRoot();
    this._rootPage.AddChild(oldRoot);
    SplitChild(this._rootPage, 0, oldRoot);
    this._bufferedPages[this._rootPage.ID] = _rootPage;
    this._bufferedPages[oldRoot.ID] = oldRoot;
    InsertNonFull(this._rootPage, newKey, data);
    this.Height++;
    UpdateLog(this._rootPage);
    WriteAllBuffersContents();
    Serialize();
}

private void InsertNonFull(NodePage<TK, TP> node, TK newKey, IEnumerable<TP> data)
{
    int positionToInsert = node.GetPosition(newKey, true);
    if (node.IsLeaf)
    {
        EntryPage<TK, TP> entry;
        if (positionToInsert < node.GetEntriesCount())
        {
            var entryToBeMoved = node.GetEntry(positionToInsert);
            entry = CreateNewEntry(newKey);
            entryToBeMoved.InsertNewEntryBetweenEntryAndPrevious(entry);
        }
        else
        {
            var previousEntry = positionToInsert - 1 >= 0 ? node.GetEntry(positionToInsert - 1) : null;
            entry = CreateNewEntry(newKey);
            if (previousEntry != null)
            {
                previousEntry.InsertNewEntryBetweenEntryAndNext(entry);
            }
        }
        entry.SetData(data);
        node.InsertEntry(positionToInsert, entry);
        this._bufferedPages[node.ID] = node;
        this._bufferedPages[entry.ID] = entry;
        return;
    }
    NodePage<TK, TP> child = node.GetChild(positionToInsert);
    if (child.HasReachedMaxEntries)
    {
        this.SplitChild(node, positionToInsert, child);
        if (newKey.CompareTo(node.GetEntry(positionToInsert).Key) > 0)
        {
            positionToInsert++;
        }
    }
    InsertNonFull(node.GetChild(positionToInsert), newKey, data);
}