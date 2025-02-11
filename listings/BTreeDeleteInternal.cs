private void DeleteInternal(NodePage<TK, TP> node, TK keyToDelete)
{
    int i = node._entries.TakeWhile(entry => keyToDelete.CompareTo(entry.Key) > 0).Count();
    if (i < node._entries.Count && node._entries[i].Key.CompareTo(keyToDelete) == 0)
    {
        this.DeleteKeyFromNode(node, keyToDelete, i);
        return;
    }
    if (!node.IsLeaf)
    {
        this.DeleteKeyFromSubtree(node, keyToDelete, i);
    }
}