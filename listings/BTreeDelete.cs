public void Delete(TK keyToDelete)
{
    this.DeleteInternal(this._rootPage, keyToDelete);
    if (this._rootPage.GetEntriesCount() == 0 && !this._rootPage.IsLeaf)
    {
        this._rootPage = this._rootPage.GetChild(0);
        this.Height--;
    }
    UpdateLog(this._rootPage);
    WriteAllBuffersContents();
    Serialize();
}
