private void SplitChild(NodePage<TK, TP> parentNode, int nodeToBeSplitIndex, NodePage<TK, TP> nodeToBeSplit)
{
    NodePage<TK, TP> newNode = CreateNewNode();

    parentNode.InsertEntry(nodeToBeSplitIndex, nodeToBeSplit.GetEntry(this.Degree - 1));
    parentNode.InsertChild(nodeToBeSplitIndex + 1, newNode);

    newNode.AppendEntries(nodeToBeSplit.GetEntriesRange(this.Degree, this.Degree - 1));

    // remove also _entries[this.Degree - 1], which is the one to move up to the parent
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