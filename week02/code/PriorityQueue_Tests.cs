using Microsoft.VisualStudio.TestTools.UnitTesting;

// Requirement-based tests, written before inspecting the PriorityQueue implementation.

[TestClass]
public class PriorityQueueTests
{
    [TestMethod]
    // Scenario: Enqueue one item and dequeue it.
    // Expected Result: The original string value is returned.
    // Original baseline: PASS. The dequeue returned "Only" and its value assertion passed.
    // This test does not check whether the item was removed; the drain test checks that.
    // Defects found: None exposed by this single-value assertion. Inspection found
    // Dequeue omitted removal, which is corrected and exercised by the drain test.
    // Post-fix: PASS. The returned value still matched "Only".
    public void TestPriorityQueue_1()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Only", 7);

        Assert.AreEqual("Only", priorityQueue.Dequeue());
    }

    [TestMethod]
    // Scenario: Enqueue mixed priorities with the highest-priority item at the end.
    // Expected Result: High (9), Medium (3), then Low (1), regardless of insertion order.
    // Original baseline: FAIL. The first assertion reported
    // "Assert.AreEqual failed. Expected:<High>. Actual:<Medium>."
    // Execution stopped there; the remaining dequeue assertions were not evaluated.
    // Defects found: The search skipped the final item, and Dequeue did not remove
    // its selected item. The search now includes the last index and removes its choice.
    // Post-fix: PASS. All three dequeue assertions matched High, Medium, Low.
    public void TestPriorityQueue_2()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Low", 1);
        priorityQueue.Enqueue("Medium", 3);
        priorityQueue.Enqueue("High", 9);

        Assert.AreEqual("High", priorityQueue.Dequeue());
        Assert.AreEqual("Medium", priorityQueue.Dequeue());
        Assert.AreEqual("Low", priorityQueue.Dequeue());
    }

    [TestMethod]
    // Scenario: Enqueue three items with identical priorities.
    // Expected Result: First, Second, Third, preserving FIFO insertion order.
    // Original baseline: FAIL. The first assertion reported
    // "Assert.AreEqual failed. Expected:<First>. Actual:<Second>."
    // Execution stopped there; the Second and Third assertions were not evaluated.
    // Defects found: A >= comparison replaced the earlier item on equal priority.
    // Using > retains the earliest tied item; removing it allows later ties to advance.
    // Post-fix: PASS. All three dequeue assertions matched First, Second, Third.
    public void TestPriorityQueue_EqualPriorities()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("First", 5);
        priorityQueue.Enqueue("Second", 5);
        priorityQueue.Enqueue("Third", 5);

        Assert.AreEqual("First", priorityQueue.Dequeue());
        Assert.AreEqual("Second", priorityQueue.Dequeue());
        Assert.AreEqual("Third", priorityQueue.Dequeue());
    }

    [TestMethod]
    // Scenario: Two highest-priority items are separated by lower-priority items.
    // Expected Result: FirstHigh, SecondHigh, Middle, Tail, Low; ties remain FIFO.
    // Original baseline: FAIL. The first assertion reported
    // "Assert.AreEqual failed. Expected:<FirstHigh>. Actual:<SecondHigh>."
    // Execution stopped there; the remaining four assertions were not evaluated.
    // Defects found: The >= comparison selected a later highest-priority tie;
    // missing removal also prevented draining. Both behaviors were corrected.
    // Post-fix: PASS. All five assertions matched FirstHigh, SecondHigh, Middle, Tail, Low.
    public void TestPriorityQueue_HighestPriorityTies()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Low", 1);
        priorityQueue.Enqueue("FirstHigh", 9);
        priorityQueue.Enqueue("Middle", 3);
        priorityQueue.Enqueue("SecondHigh", 9);
        priorityQueue.Enqueue("Tail", 2);

        Assert.AreEqual("FirstHigh", priorityQueue.Dequeue());
        Assert.AreEqual("SecondHigh", priorityQueue.Dequeue());
        Assert.AreEqual("Middle", priorityQueue.Dequeue());
        Assert.AreEqual("Tail", priorityQueue.Dequeue());
        Assert.AreEqual("Low", priorityQueue.Dequeue());
    }

    [TestMethod]
    // Scenario: Drain a queue containing two items and then dequeue once more.
    // Expected Result: High then Low are removed; another dequeue throws
    // InvalidOperationException with the exact message "The queue is empty."
    // Original baseline: FAIL. The first dequeue matched High; the second reported
    // "Assert.AreEqual failed. Expected:<Low>. Actual:<High>."
    // Execution stopped there; the empty-queue exception and message were not checked.
    // Defects found: Dequeue returned the selected value without removing its item.
    // It now removes the item before returning, allowing the queue to become empty.
    // Post-fix: PASS. High and Low were returned once each, followed by the required
    // InvalidOperationException and exact message "The queue is empty."
    public void TestPriorityQueue_DrainRemovesItems()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("High", 8);
        priorityQueue.Enqueue("Low", 2);

        Assert.AreEqual("High", priorityQueue.Dequeue());
        Assert.AreEqual("Low", priorityQueue.Dequeue());
        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", exception.Message);
    }

    [TestMethod]
    // Scenario: Dequeue a high item, add more items including tied priorities,
    // dequeue again, and add a new item tied with an item already waiting.
    // Expected Result: First is removed before the new enqueues; then High,
    // Second, Third, Fourth, Low follow, preserving priority and FIFO tie order.
    // Original baseline: FAIL. The First and High assertions passed; the next reported
    // "Assert.AreEqual failed. Expected:<Second>. Actual:<High>."
    // Execution stopped there; Fourth was not enqueued and later assertions were not run.
    // Defects found: Missing removal left High available again. The search also needed
    // to include its final item and preserve the earliest equal-priority item using >.
    // Post-fix: PASS. All six name assertions passed across the interleaved operations:
    // First, High, Second, Third, Fourth, Low.
    public void TestPriorityQueue_InterleavedOperations()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("First", 5);
        priorityQueue.Enqueue("Low", 1);
        Assert.AreEqual("First", priorityQueue.Dequeue());

        priorityQueue.Enqueue("Second", 5);
        priorityQueue.Enqueue("High", 8);
        priorityQueue.Enqueue("Third", 5);
        Assert.AreEqual("High", priorityQueue.Dequeue());
        Assert.AreEqual("Second", priorityQueue.Dequeue());

        priorityQueue.Enqueue("Fourth", 5);
        Assert.AreEqual("Third", priorityQueue.Dequeue());
        Assert.AreEqual("Fourth", priorityQueue.Dequeue());
        Assert.AreEqual("Low", priorityQueue.Dequeue());
    }

    [TestMethod]
    // Scenario: Enqueue zero and negative priorities in a mixed order.
    // Expected Result: Zero (0), Middle (-1), Negative (-5), Lower (-10).
    // Original baseline: FAIL. The first dequeue matched Zero; the second reported
    // "Assert.AreEqual failed. Expected:<Middle>. Actual:<Zero>."
    // Execution stopped there; the Negative and Lower assertions were not evaluated.
    // Defects found: Missing removal returned Zero again; the truncated search would
    // also miss Middle at the end. Comparing every remaining item fixes that omission.
    // Post-fix: PASS. All four assertions matched Zero, Middle, Negative, Lower.
    public void TestPriorityQueue_ZeroAndNegativePriorities()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Negative", -5);
        priorityQueue.Enqueue("Zero", 0);
        priorityQueue.Enqueue("Lower", -10);
        priorityQueue.Enqueue("Middle", -1);

        Assert.AreEqual("Zero", priorityQueue.Dequeue());
        Assert.AreEqual("Middle", priorityQueue.Dequeue());
        Assert.AreEqual("Negative", priorityQueue.Dequeue());
        Assert.AreEqual("Lower", priorityQueue.Dequeue());
    }

    [TestMethod]
    // Scenario: Dequeue from a newly created, empty queue.
    // Expected Result: InvalidOperationException with exactly "The queue is empty."
    // Original baseline: PASS. Both the exception type and exact message assertions passed.
    // Defects found: None in the existing empty-queue guard; it was preserved.
    // Post-fix: PASS. Both the exception type and exact message assertions passed again.
    public void TestPriorityQueue_InitiallyEmpty()
    {
        var priorityQueue = new PriorityQueue();

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", exception.Message);
    }
}
