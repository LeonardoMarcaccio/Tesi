private void DeleteKeyFromNode(NodePage<TK, TP> node, TK keyToDelete, int keyIndexInNode)
{
    if (node.IsLeaf)
    {
        node._entries.RemoveAt(keyIndexInNode);
        return;
    }
    NodePage<TK, TP> predecessorChild = node._children[keyIndexInNode]._nodePage;
    if (predecessorChild._entries.Count >= this.Degree)
    {
        EntryPage<TK, TP> predecessorEntry = GetLastEntry(predecessorChild);
        DeleteInternal(predecessorChild, predecessorEntry.Key);
        node._entries[keyIndexInNode] = new NodePage<TK, TP>.EntryWithPage(predecessorEntry);
    }
    else
    {
        NodePage<TK, TP> successorChild = node._children[keyIndexInNode + 1]._nodePage;
        if (successorChild._entries.Count >= this.Degree)
        {
            EntryPage<TK, TP> successorEntry = GetFirstEntry(successorChild);
            DeleteInternal(successorChild, successorEntry.Key);
            node._entries[keyIndexInNode] = new NodePage<TK, TP>.EntryWithPage(successorEntry);
        }
        else
        {
            predecessorChild._entries.Add(node._entries[keyIndexInNode]);
            predecessorChild._entries.AddRange(successorChild._entries);
            predecessorChild._children.AddRange(successorChild._children);
            node._entries.RemoveAt(keyIndexInNode);
            node._children.RemoveAt(keyIndexInNode + 1);
            this.DeleteInternal(predecessorChild, keyToDelete);
        }
    }
}