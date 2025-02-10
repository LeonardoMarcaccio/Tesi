internal void AddData(TP data, byte[] dataBuffer, int position)
{
    if (dataBuffer.Length > this.FreePageSize)
    {
        throw new ArgumentException($"dataBuffer.Length > FreePageSize, dataBuffer.Length: {dataBuffer.Length}, FreePageSize: {this.FreePageSize}");
    }
    this._data.Insert(position, data);
    UpdateDataInBuffer();
}

internal void SetData(IEnumerable<TP> enumerableData)
{
    List<TP> dataToInsert = enumerableData.ToList();
    if (dataToInsert.Count * this._dataToBuffer.SizeInBytes() > GetMaxtFreePageSize())
    {
        throw new ArgumentException(
            $"dataBuffer.Count * SysConstants.PAGE_SIZE > FreePageSize, dataBuffer.Length * SysConstants.PAGE_SIZE: {dataToInsert.Count * SysConstants.PAGE_SIZE}, FreePageSize: {this.FreePageSize}"
        );
    }
    this._data = dataToInsert;
    UpdateDataInBuffer();
}

internal int FindPositionToInsert(TP data)
{
    return _data.TakeWhile(dataInEntry => data.CompareTo(dataInEntry) >= 0).Count();
}
