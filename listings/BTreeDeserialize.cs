internal EntryPage<TK, TP>? DeserializeEntryFromID(uint ID)
{
    this._fileStream.Seek(ID * SysConstants.PAGE_SIZE, SeekOrigin.Begin);
    byte[] buffer = new byte[SysConstants.PAGE_SIZE];
    this._fileStream.Read(buffer, 0, SysConstants.PAGE_SIZE);
    if (buffer[4] == 3)
    {
        return new EntryPage<TK, TP>(
            this,
            this._entryKeyByteConverter,
            this._dataToBuffer,
            this._keyNullIndicator,
            buffer
        );
    }
    else
    {
        return null;
    }
}
