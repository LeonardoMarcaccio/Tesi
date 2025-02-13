private void CreateNewRoot()
{
    this._rootPage = new NodePage<TK, TP>(this, this.Degree, this._entryKeyByteConverter, this._dataToBuffer, this._keyNullIndicator, this._bootPage.GetNewPageId());
    UpdateBootRootID(this._rootPage.ID);
    this._loggedPages.TryAdd(this._bootPage.ID, this._bootPage);
    this._bufferedPages.TryAdd(this._bootPage.ID, this._bootPage);
}

private NodePage<TK, TP> CreateNewNode()
{
    this._loggedPages.TryAdd(this._bootPage.ID, this._bootPage);
    this._bufferedPages.TryAdd(this._bootPage.ID, this._bootPage);
    uint nodeId = this._bootPage.GetNewPageId();
    return new NodePage<TK, TP>(this, this.Degree, this._entryKeyByteConverter, this._dataToBuffer, this._keyNullIndicator, nodeId);
}

private EntryPage<TK, TP> CreateNewEntry(TK key)
{
    this._loggedPages.TryAdd(this._bootPage.ID, this._bootPage);
    this._bufferedPages.TryAdd(this._bootPage.ID, this._bootPage);
    uint entryId = this._bootPage.GetNewPageId();
    return new EntryPage<TK, TP>( this, this._entryKeyByteConverter, this._dataToBuffer, this._keyNullIndicator, entryId, key, null, null);
}
