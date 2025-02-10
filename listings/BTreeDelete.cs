
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

    private void DeleteKeyFromSubtree(NodePage<TK, TP> parentNode, TK keyToDelete, int subtreeIndexInNode)
    {
        NodePage<TK, TP> childNode = parentNode._children[subtreeIndexInNode]._nodePage;
        if (childNode.HasReachedMinEntries)
        {
            int leftIndex = subtreeIndexInNode == 0 ? 0 : subtreeIndexInNode - 1;
            NodePage<TK, TP>? leftSibling = subtreeIndexInNode > 0
                                            ? parentNode._children[leftIndex]._nodePage
                                            : null;

            int rightIndex = subtreeIndexInNode + 1;
            NodePage<TK, TP>? rightSibling = subtreeIndexInNode < parentNode._children.Count - 1
                                            ? parentNode._children[rightIndex]._nodePage
                                            : null;

            if (leftSibling != null && leftSibling._entries.Count > this.Degree - 1)
            {
                childNode._entries.Insert(0, parentNode._entries[leftIndex]);
                parentNode._entries[leftIndex] = leftSibling._entries.Last();
                leftSibling._entries.RemoveAt(leftSibling._entries.Count - 1);
                if (!leftSibling.IsLeaf)
                {
                    childNode._children.Insert(0, leftSibling._children.Last());
                    leftSibling._children.RemoveAt(leftSibling._children.Count - 1);
                }
                childNode.SetModified(true);
                this._bufferedPages.TryAdd(childNode.ID, childNode);
                this._loggedPages.TryAdd(childNode.ID, childNode);
                parentNode.SetModified(true);
                this._bufferedPages.TryAdd(parentNode.ID, parentNode);
                this._loggedPages.TryAdd(parentNode.ID, parentNode);
                leftSibling.SetModified(true);
                this._bufferedPages.TryAdd(leftSibling.ID, leftSibling);
                this._loggedPages.TryAdd(leftSibling.ID, leftSibling);
            }
            else if (rightSibling != null && rightSibling._entries.Count > this.Degree - 1)
            {
                childNode._entries.Add(parentNode._entries[subtreeIndexInNode]);
                parentNode._entries[subtreeIndexInNode] = rightSibling._entries.First();
                rightSibling._entries.RemoveAt(0);
                if (!rightSibling.IsLeaf)
                {
                    childNode._children.Add(rightSibling._children.First());
                    rightSibling._children.RemoveAt(0);
                }
                childNode.SetModified(true);
                this._bufferedPages.TryAdd(childNode.ID, childNode);
                this._loggedPages.TryAdd(childNode.ID, childNode);
                parentNode.SetModified(true);
                this._bufferedPages.TryAdd(parentNode.ID, parentNode);
                this._loggedPages.TryAdd(parentNode.ID, parentNode);
                rightSibling.SetModified(true);
                this._bufferedPages.TryAdd(rightSibling.ID, rightSibling);
                this._loggedPages.TryAdd(rightSibling.ID, rightSibling);
            }
            else
            {
                if (leftSibling != null)
                {
                    childNode._entries.Insert(0, parentNode._entries[leftIndex]);
                    List<EntryPage<TK, TP>> oldEntries = childNode._entries.Select(entry => entry._entryPage).ToList();
                    childNode.SetEntries(leftSibling.GetAllEntries());
                    childNode.AppendEntries(oldEntries);
                    if (!leftSibling.IsLeaf)
                    {
                        var oldChildren = childNode._children;
                        childNode.SetChildren(leftSibling._children.Select(child => child._nodePage).ToList());
                        childNode._children.AddRange(oldChildren);
                    }
                    parentNode._children.RemoveAt(leftIndex);
                    parentNode._entries.RemoveAt(leftIndex);
                    childNode.SetModified(true);
                    this._bufferedPages.TryAdd(childNode.ID, childNode);
                    this._loggedPages.TryAdd(childNode.ID, childNode);
                    parentNode.SetModified(true);
                    this._bufferedPages.TryAdd(parentNode.ID, parentNode);
                    this._loggedPages.TryAdd(parentNode.ID, parentNode);
                    leftSibling.SetModified(true);
                    this._bufferedPages.TryAdd(leftSibling.ID, leftSibling);
                    this._loggedPages.TryAdd(leftSibling.ID, leftSibling);
                }
                else
                {
                    Debug.Assert(rightSibling != null, "Node should have at least one sibling");
                    childNode._entries.Add(parentNode._entries[subtreeIndexInNode]);
                    childNode._entries.AddRange(rightSibling._entries);
                    if (!rightSibling.IsLeaf)
                    {
                        childNode._children.AddRange(rightSibling._children);
                    }
                    parentNode._children.RemoveAt(rightIndex);
                    parentNode._entries.RemoveAt(subtreeIndexInNode);
                    childNode.SetModified(true);
                    this._bufferedPages.TryAdd(childNode.ID, childNode);
                    this._loggedPages.TryAdd(childNode.ID, childNode);
                    parentNode.SetModified(true);
                    this._bufferedPages.TryAdd(parentNode.ID, parentNode);
                    this._loggedPages.TryAdd(parentNode.ID, parentNode);
                    rightSibling.SetModified(true);
                    this._bufferedPages.TryAdd(rightSibling.ID, rightSibling);
                    this._loggedPages.TryAdd(rightSibling.ID, rightSibling);
                }
            }
        }
        this.DeleteInternal(childNode, keyToDelete);
    }

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



    private EntryPage<TK, TP> GetLastEntry(NodePage<TK, TP> node)
    {
        if (node.IsLeaf)
        {
            return node.Entries.Last()._entryPage;
        }

        return this.GetLastEntry(
            node.Children.Last()._nodePage
        );
    }

    private EntryPage<TK, TP> GetFirstEntry(NodePage<TK, TP> node)
    {
        if (node.IsLeaf)
        {
            return node.Entries.First()._entryPage;
        }

        return this.GetFirstEntry(
            node.Children.First()._nodePage
        );
    }

    private void SplitChild(NodePage<TK, TP> parentNode, int nodeToBeSplitIndex, NodePage<TK, TP> nodeToBeSplit)
    {
        NodePage<TK, TP> newNode = CreateNewNode();

        parentNode.InsertEntry(nodeToBeSplitIndex, nodeToBeSplit.GetEntry(this.Degree - 1));
        parentNode.InsertChild(nodeToBeSplitIndex + 1, newNode);

        newNode.AppendEntries(nodeToBeSplit.GetEntriesRange(this.Degree, this.Degree - 1));

        nodeToBeSplit.RemoveEntriesRange(this.Degree - 1, this.Degree);

        if (!nodeToBeSplit.IsLeaf)
        {
            newNode.AppendChildren(nodeToBeSplit.GetChildrenRange(this.Degree, this.Degree));
            nodeToBeSplit.RemoveChildrenRange(this.Degree, this.Degree);
        }

        parentNode.UpdateEntryReferences();
        newNode.UpdateEntryReferences();
        nodeToBeSplit.UpdateEntryReferences();
    }

    private void UpdateLog(NodePage<TK, TP> node)
    {
        //if (node.IsModified)
        //{
        //    this._loggedPages.TryAdd(node.ID, node);
        //    this._bufferedPages.TryAdd(node.ID, node);
        //}

        //foreach (var entry in node._entries)
        //{
        //    if (entry._entryPage.IsModified)
        //    {
        //        this._loggedPages.TryAdd(entry.EntryPageID, entry._entryPage);
        //        this._bufferedPages.TryAdd(entry.EntryPageID, entry._entryPage);
        //    }
        //}

        //if (!node.IsLeaf)
        //{
        //    foreach (var child in node._children)
        //    {
        //        UpdateLog(child._nodePage);
        //    }
        //}
    }

    private void WriteAllBuffersContents()
    {
        foreach (var page in this._bufferedPages.Values)
        {
            page.Write();
        }

        foreach (var page in this._loggedPages.Values)
        {
            page.Write();
        }
    }

    private void Serialize()
    {
        try
        {
            foreach (var page in this._loggedPages.Values)
            {
                byte[] tmp = new byte[SysConstants.PAGE_SIZE];
                page.Write(tmp);
                this._logFileStream.Write(tmp); //Cast Necessario
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

    public byte[] GetFileSystemBuffer()
    {
        var memoryStream = new MemoryStream();
        this._fileStream.Position = 0;
        this._fileStream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
