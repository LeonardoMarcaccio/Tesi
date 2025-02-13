public static void RunBruteForce(int degree)
{
    var btree = new BTree<string, int>(degree);
    var rand = new Random();
    for (int i = 0; i < 1000; i++)
    {
        var value = (int)rand.Next() % 100;
        var key = value.ToString();
        if (rand.Next() % 2 == 0)
        {
            if (btree.Search(key) == null)
            {
                btree.Insert(key, value);
            }
            Assert.True(btree.Search(key).Pointer == value);
        }
        else
        {
            btree.Delete(key);
            Assert.Null(btree.Search(key));
        }
        CheckNode(btree.Root, degree);
    }
}

public static void CheckNode(NodePage<string, int> node, int degree)
{
    if (node.Children.Count > 0 &&
    node.Children.Count != node.Entries.Count + 1)
    {
        Assert.Fail("There are children, but they don't match the number of entries.");
    }
    if (node.Entries.Count > (2 * degree) - 1)
    {
        Assert.Fail("Too much entries in node");
    }
    if (node.Children.Count > degree * 2)
    {
        Assert.Fail("Too much children in node");
    }
    foreach (var child in node.Children)
    {
        CheckNode(child, degree);
    }
}
