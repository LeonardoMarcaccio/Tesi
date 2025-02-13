public class Page
{
    public uint ID { get; protected set; }
    public byte Type { get; protected set; }
    public byte[] Buffer { get; protected set; }
    public int FreePageSize { get; protected set; }
    public bool IsModified { get; protected set; }

    public Page(uint Id, byte Type)
    {
        this._buffer = new byte[SysConstants.PAGE_SIZE];
        this.ID = Id;
        Array.Copy(
            BitConverter.GetBytes(ID),
            0,
            this._buffer,
            0,
            SysConstants.PAGE_ID_SIZE
        );
        this.Type = Type;
        this._buffer[4] = this.Type;
        this.FreePageSize = SysConstants.PAGE_SIZE - SysConstants.HEADER_SIZE;
        this.SetModified(true);
    }

    public Page(byte[] buffer)
    {
        this._buffer = buffer;
        this.FreePageSize = SysConstants.PAGE_SIZE - SysConstants.HEADER_SIZE;
        this.SetModified(true);
    }

    public virtual void Write()
    {
        Write(this._buffer);
    }

    public virtual void Write(byte[] buffer)
    {
        this.SetModified(false);
        Array.Copy(BitConverter.GetBytes(ID), 0, buffer, 0, SysConstants.PAGE_ID_SIZE);
        buffer[SysConstants.PAGE_ID_SIZE] = this.Type;
    }

    public virtual void Read()
    {
        this.FreePageSize = SysConstants.PAGE_SIZE - SysConstants.HEADER_SIZE;
        this.ID = BitConverter.ToUInt32(_buffer, 0);
        Type = _buffer[SysConstants.PAGE_ID_SIZE];
    }

    public void SetModified(bool val)
    {
        this.IsModified = val;
    }
}
