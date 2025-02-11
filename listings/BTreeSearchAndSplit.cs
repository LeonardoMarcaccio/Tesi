private void SplitChild(NodePage<TK, TP> parentNode, int nodeToBeSplitIndex, NodePage<TK, TP> nodeToBeSplit)
{
    NodePage<TK, TP> newNode = CreateNewNode();
    parentNode.InsertEntry(nodeToBeSplitIndex, nodeToBeSplit.GetEntry(this.Degree - 1));
    parentNode.InsertChild(nodeToBeSplitIndex + 1, newNode);
    newNode.AppendEntries(nodeToBeSplit.GetEntriesRange(this.Degree, this.Degree - 1));
    nodeToBeSplit.RemoveEntriesRange(this.Degree - 1, this.Degree);
    if (!nodeToBeSplit.IsLeaf)
    {
        newNode.AppendChildren(nodeToBeSplit.GetChildrenRange(this.Degree, this.Degree));
        nodeToBeSplit.RemoveChildrenRange(this.Degree, this.Degree);
    }
    parentNode.UpdateEntryReferences();
    newNode.UpdateEntryReferences();
    nodeToBeSplit.UpdateEntryReferences();
}

internal (Page? Page, byte[] PageBuffer) GetPage(uint pageID)
{
    if (BufferedPages.TryGetValue(pageID, out var page))
    {
        return (page, new byte[0]);
    }

    byte[] buffer = new byte[SysConstants.PAGE_SIZE];
    long pageStart = SysConstants.PAGE_SIZE * pageID;

    if (this._fileStream.Length < pageStart + SysConstants.PAGE_SIZE)
    {
        throw new ArgumentOutOfRangeException($"GetPage, pageNumber: {pageID}, FileStream.Length: {this._fileStream.Length}, PAGE_SIZE: {SysConstants.PAGE_SIZE}");
    }

    this._fileStream.Position = pageStart;
    this._fileStream.Read(buffer, 0, SysConstants.PAGE_SIZE);
    return (null, buffer);
}
