using System.Globalization;
using System.Text.Json;

public static class SetsAndMaps
{
    /// <summary>
    /// The words parameter contains a list of two character 
    /// words (lower case, no duplicates). Using sets, find an O(n) 
    /// solution for returning all symmetric pairs of words.  
    ///
    /// For example, if words was: [am, at, ma, if, fi], we would return :
    ///
    /// ["am & ma", "if & fi"]
    ///
    /// The order of the array does not matter, nor does the order of the specific words in each string in the array.
    /// at would not be returned because ta is not in the list of words.
    ///
    /// As a special case, if the letters are the same (example: 'aa') then
    /// it would not match anything else (remember the assumption above
    /// that there were no duplicates) and therefore should not be returned.
    /// </summary>
    /// <param name="words">An array of 2-character words (lowercase, no duplicates)</param>
    public static string[] FindPairs(string[] words)
    {
        var seen = new HashSet<string>();
        var pairs = new List<string>();
        foreach (var word in words)
        {
            // Only the later, distinct word in a reversed pair produces a result.
            if (word[0] == word[1] || !seen.Add(word))
                continue;

            var reverse = $"{word[1]}{word[0]}";
            if (seen.Contains(reverse))
                pairs.Add($"{word} & {reverse}");
        }

        return pairs.ToArray();
    }

    /// <summary>
    /// Read a census file and summarize the degrees (education)
    /// earned by those contained in the file.  The summary
    /// should be stored in a dictionary where the key is the
    /// degree earned and the value is the number of people that 
    /// have earned that degree.  The degree information is in
    /// the 4th column of the file.  There is no header row in the
    /// file.
    /// </summary>
    /// <param name="filename">The name of the file to read</param>
    /// <returns>fixed array of divisors</returns>
    public static Dictionary<string, int> SummarizeDegrees(string filename)
    {
        var degrees = new Dictionary<string, int>();
        foreach (var line in File.ReadLines(filename))
        {
            var fields = line.Split(",");
            var degree = fields[3].Trim();
            // Discover each degree as it appears, then count its occurrences.
            degrees.TryGetValue(degree, out var count);
            degrees[degree] = count + 1;
        }

        return degrees;
    }

    /// <summary>
    /// Determine if 'word1' and 'word2' are anagrams.  An anagram
    /// is when the same letters in a word are re-organized into a 
    /// new word.  A dictionary is used to solve the problem.
    /// 
    /// Examples:
    /// is_anagram("CAT","ACT") would return true
    /// is_anagram("DOG","GOOD") would return false because GOOD has 2 O's
    /// 
    /// Important Note: When determining if two words are anagrams, you
    /// should ignore any spaces.  You should also ignore cases.  For 
    /// example, 'Ab' and 'Ba' should be considered anagrams
    /// 
    /// Reminder: You can access a letter by index in a string by 
    /// using the [] notation.
    /// </summary>
    public static bool IsAnagram(string word1, string word2)
    {
        var counts = new Dictionary<char, int>();
        foreach (var character in word1)
        {
            if (character == ' ')
                continue;

            var letter = char.ToUpperInvariant(character);
            counts.TryGetValue(letter, out var count);
            counts[letter] = count + 1;
        }

        // Consume one matching occurrence for each non-space character in word2.
        foreach (var character in word2)
        {
            if (character == ' ')
                continue;

            var letter = char.ToUpperInvariant(character);
            if (!counts.TryGetValue(letter, out var count))
                return false;

            if (count == 1)
                counts.Remove(letter);
            else
                counts[letter] = count - 1;
        }

        return counts.Count == 0;
    }

    /// <summary>
    /// This function will read JSON (Javascript Object Notation) data from the 
    /// United States Geological Service (USGS) consisting of earthquake data.
    /// The data will include all earthquakes in the current day.
    /// 
    /// JSON data is organized into a dictionary. After reading the data using
    /// the built-in HTTP client library, this function will return a list of all
    /// earthquake locations ('place' attribute) and magnitudes ('mag' attribute).
    /// Additional information about the format of the JSON data can be found 
    /// at this website:  
    /// 
    /// https://earthquake.usgs.gov/earthquakes/feed/v1.0/geojson.php
    /// 
    /// </summary>
    public static string[] EarthquakeDailySummary()
    {
        const string uri = "https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_day.geojson";
        using var client = new HttpClient();
        using var getRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        using var response = client.Send(getRequestMessage);
        response.EnsureSuccessStatusCode();
        using var jsonStream = response.Content.ReadAsStream();
        using var reader = new StreamReader(jsonStream);
        var json = reader.ReadToEnd();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var featureCollection = JsonSerializer.Deserialize<FeatureCollection>(json, options);

        var features = featureCollection?.Features
            ?? throw new JsonException("Earthquake feed is missing its features array.");
        var summaries = new string[features.Length];
        for (var i = 0; i < features.Length; ++i)
        {
            var properties = features[i]?.Properties
                ?? throw new JsonException("Earthquake feature is missing its properties.");
            // Nullable feed values stay explicitly unknown instead of becoming zero.
            var place = properties.Place ?? "Unknown location";
            var magnitude = properties.Mag?.ToString(CultureInfo.InvariantCulture) ?? "unknown";
            summaries[i] = $"{place} - Mag {magnitude}";
        }

        return summaries;
    }
}
