private void Serialize()
{
    try
    {
        foreach (var page in this._loggedPages.Values)
        {
            byte[] tmp = new byte[SysConstants.PAGE_SIZE];
            page.Write(tmp);
            this._logFileStream.Write(tmp);
        }
    }
    catch (Exception ex)
    {
        throw new Exception("Unable to create a new log");
    }
    this._logFileStream.Dispose();
    try
    {
        this._logFileStream = new FileStream(
            Path.Combine(this._folderPath, this._bootPage.GetNewLogId() + ".fslog"),
            FileMode.OpenOrCreate,
            FileAccess.ReadWrite,
            FileShare.Read
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine("Throwed exception : " + ex.Message + "in filestream or logstream opening");
    }
    this._loggedPages = new SortedDictionary<uint, Page>();
    try
    {
        foreach (var page in this._bufferedPages.Values)
        {
            byte[] tmp = new byte[SysConstants.PAGE_SIZE];
            page.Write(tmp);
            this._fileStream.Seek(page.ID * SysConstants.PAGE_SIZE, SeekOrigin.Begin);
            this._fileStream.Write(tmp);
        }
    }
    catch
    {
        throw new Exception("Unable to create a new save");
    }
    this._bufferedPages = new SortedDictionary<uint, Page>();
}
