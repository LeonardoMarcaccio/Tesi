public class ChildWithPage
{
    public uint ChildPageId { get; set; }
    internal NodePage<TK, TP> _nodePage;
    public NodePage<TK, TP> NodePage
    {
        get
        {
            byte[] buffer = this._nodePage.Buffer;
            this._nodePage.Write(buffer);
            return new NodePage<TK, TP>(
                this._nodePage._BTree,
                this._nodePage._entryKeyByteConverter,
                this._nodePage._dataToBuffer,
                this._nodePage._keyNullIndicator,
                buffer
            );
        }
    }

    public ChildWithPage(uint childPageId)
    {
        this.ChildPageId = childPageId;
    }

    public ChildWithPage(NodePage<TK, TP> nodePage) : this(nodePage.ID)
    {
        this._nodePage = nodePage;
    }
}

private NodePage<TK, TP> CheckChildPage(ChildWithPage child)
{
    if (child._nodePage != null)
    {
        return child._nodePage;
    }
    if (this._BTree.BufferedPages.TryGetValue(child.ChildPageId, out var bufferedNodePage))
    {
        child._nodePage = bufferedNodePage as NodePage<TK, TP>;
    }
    else
    {
        var childNodePageOrBuffer = _BTree.GetPage(child.ChildPageId);
        child._nodePage = childNodePageOrBuffer.Page != null
            ? childNodePageOrBuffer.Page as NodePage<TK, TP>
            : new NodePage<TK, TP>(
                this._BTree,
                this._entryKeyByteConverter,
                this._dataToBuffer,
                this._keyNullIndicator,
                childNodePageOrBuffer.PageBuffer
            );
        this._BTree.BufferedPages[child.ChildPageId] = child._nodePage;
    }
    return child._nodePage;
}
