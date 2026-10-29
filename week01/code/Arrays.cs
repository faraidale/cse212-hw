public static class Arrays
{
    /// <summary>
    /// This function will produce an array of size 'length' starting with 'number' followed by multiples of 'number'.  For 
    /// example, MultiplesOf(7, 5) will result in: {7, 14, 21, 28, 35}.  Assume that length is a positive
    /// integer greater than 0.
    /// </summary>
    /// <returns>array of doubles that are the multiples of the supplied number</returns>
    public static double[] MultiplesOf(double number, int length)
    {
        // Plan:
        // 1. Allocate a double array with exactly length elements to hold the multiples.
        // 2. Loop through every array index, starting at 0 and ending at length - 1.
        // 3. At each index, multiply number by (index + 1) and store the result there.
        //    This makes the first element number instead of zero and uses multiplication
        //    rather than repeatedly adding number.
        // 4. Return the completed array after the loop finishes.

        double[] multiples = new double[length];
        for (int index = 0; index < length; index++)
        {
            multiples[index] = number * (index + 1);
        }

        return multiples;
    }

    /// <summary>
    /// Rotate the 'data' to the right by the 'amount'.  For example, if the data is 
    /// List<int>{1, 2, 3, 4, 5, 6, 7, 8, 9} and an amount is 3 then the list after the function runs should be 
    /// List<int>{7, 8, 9, 1, 2, 3, 4, 5, 6}.  The value of amount will be in the range of 1 to data.Count, inclusive.
    ///
    /// Because a list is dynamic, this function will modify the existing data list rather than returning a new list.
    /// </summary>
    public static void RotateListRight(List<int> data, int amount)
    {
        // Plan:
        // 1. Calculate the starting index of the final amount elements as data.Count - amount.
        // 2. Use GetRange with that starting index and amount to copy the final elements
        //    into a temporary list, preserving their existing order.
        // 3. Use RemoveRange with the same starting index and amount to remove those
        //    elements from the original list.
        // 4. Use InsertRange to insert the saved elements at index 0 of the original list.
        //    This moves the final segment to the front while preserving both segments' order.
        //    Keep modifying the original list object instead of reassigning the parameter.
        //    If amount equals data.Count, the whole list is saved, removed, and reinserted
        //    in its original order.
        int startIndex = data.Count - amount;
        List<int> finalElements = data.GetRange(startIndex, amount);
        data.RemoveRange(startIndex, amount);
        data.InsertRange(0, finalElements);
    }
}
