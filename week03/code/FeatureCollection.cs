public class FeatureCollection
{
    public Feature[] Features { get; set; }
}

public class Feature
{
    public EarthquakeProperties Properties { get; set; }
}

public class EarthquakeProperties
{
    // USGS may supply null for the place or magnitude.
    public string Place { get; set; }
    public double? Mag { get; set; }
}
