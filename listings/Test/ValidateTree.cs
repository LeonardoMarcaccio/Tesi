public static void ValidateTree(NodePage<int, int> treeroot, int degree, params int[] expectedKeys)
{
    var foundKeys = new Dictionary<int, List<EntryPage<int, int>>>();
    ValidateSubtree(treeroot, treeroot, degree, int.MinValue, int.MaxValue, foundKeys);
    Assert.True(expectedKeys.Except(foundKeys.Keys).Count() == 0);
    foreach (var keyValuePair in foundKeys)
    {
        Assert.True(keyValuePair.Value.Count == 1);
    }
}

public static void ValidateSubtree(NodePage<int, int> root, NodePage<int, int> node, int degree, int nodeMin, int nodeMax, Dictionary<int, List<EntryPage<int, int>>> foundKeys)
{
    if (root != node)
    {
        Assert.True(node.Entries.Count >= degree - 1);
        Assert.True(node.Entries.Count <= (2 * degree) - 1);
    }
    for (int i = 0; i <= node.Entries.Count; i++)
    {
        int subtreeMin = nodeMin;
        int subtreeMax = nodeMax;
        if (i < node.Entries.Count)
        {
            var entry = node.Entries[i];
            UpdateFoundKeys(foundKeys, entry);
            Assert.True(entry.Key >= nodeMin && entry.Key <= nodeMax);

            subtreeMax = entry.Key;
        }
        if (i > 0)
        {
            subtreeMin = node.Entries[i - 1].Key;
        }
        if (!node.IsLeaf)
        {
            Assert.True(node.Children.Count >= degree);
            ValidateSubtree(root, node.Children[i], degree, subtreeMin, subtreeMax, foundKeys);
        }
    }
}
