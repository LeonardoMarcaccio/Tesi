public class BootPage<TK, TP> : Page where TK : IComparable<TK> where TP : IComparable<TP>
{
    private BTree<TK, TP> _BTree;
    public uint LastPageId { get; }
    public ulong LastLogVersionId { get; }
    public List<uint> EmptyPageIndexes { get; }
    public uint RootPageID { get; }

    public BootPage(BTree<TK, TP> BTree, uint Id) : base(Id, PageType.BOOT)
    {
        this._BTree = BTree;
        this._lastPageId = Id;
        Array.Copy(BitConverter.GetBytes(this._lastPageId), 0, this._buffer, 0, SysConstants.PAGE_ID_SIZE);
        this._lastLogVersion = 0;
        Array.Copy(BitConverter.GetBytes(this._lastLogVersion), 0, this._buffer, 0, SysConstants.PAGE_ID_SIZE);
        this._emptyPageIndexes = new List<uint>();
    }

    public BootPage(BTree<TK, TP> BTree, byte[] buffer) : base(buffer)
    {
        this._BTree = BTree;
        Read();
    }

    public void AddID(uint ID)
    {
        this._emptyPageIndexes.Add(ID);
        this._emptyPageIndexes.Sort();
    }

    public uint GetNewPageId()
    {
        if (this._emptyPageIndexes.Any())
        {
            uint index = this._emptyPageIndexes.First();
            return index;
        }

        this._lastPageId++;
        this.SetModified(true);
        this._BTree._bufferedPages.TryAdd(this.ID, this);
        this._BTree._loggedPages.TryAdd(this.ID, this);
        Write();
        return this.LastPageId;
    }

    public ulong GetNewLogId()
    {
        this.SetModified(true);
        this._BTree._bufferedPages.TryAdd(this.ID, this);
        this._BTree._loggedPages.TryAdd(this.ID, this);
        return this._lastLogVersion++;
    }
}
