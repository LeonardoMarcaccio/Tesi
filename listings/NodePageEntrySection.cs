public class EntryWithPage
{
    public TK Key { get; set; }
    public uint EntryPageID { get; set; }
    internal EntryPage<TK, TP> _entryPage;
    public EntryPage<TK, TP> EntryPage
    {
        get
        {
            return new EntryPage<TK, TP>(
                this._entryPage._BTree,
                this._entryPage._entryKeyByteConverter,
                this._entryPage._dataToBuffer,
                this._entryPage._keyNullIndicator,
                this._entryPage.Buffer
            );
        }
    }

    public EntryWithPage(uint entryPageId, TK key)
    {
        EntryPageID = entryPageId;
        Key = key;
    }

    public EntryWithPage(EntryPage<TK, TP> entryPage) : this(entryPage.ID, entryPage.Key)
    {
        _entryPage = entryPage;
    }
}

private EntryPage<TK, TP> CheckEntryPage(EntryWithPage entryWithPage)
{
    if (entryWithPage._entryPage != null)
    {
        return entryWithPage._entryPage;
    }
    if (_BTree.BufferedPages.TryGetValue(entryWithPage.EntryPageID, out var bufferedEntryPage))
    {
        entryWithPage._entryPage = bufferedEntryPage as EntryPage<TK, TP>;
    }
    else
    {
        var entryPageOrBuffer = _BTree.GetPage(entryWithPage.EntryPageID);
        entryWithPage._entryPage = entryPageOrBuffer.Page != null
            ? entryPageOrBuffer.Page as EntryPage<TK, TP>
            : new EntryPage<TK, TP>(
                this._BTree,
                this._entryKeyByteConverter,
                this._dataToBuffer,
                this._keyNullIndicator,
                entryPageOrBuffer.PageBuffer
            );
        _BTree.BufferedPages[entryWithPage.EntryPageID] = entryWithPage._entryPage;
    }
    return entryWithPage._entryPage;
}
