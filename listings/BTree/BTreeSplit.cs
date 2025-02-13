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
