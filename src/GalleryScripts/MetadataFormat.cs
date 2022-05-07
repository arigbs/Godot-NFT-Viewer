using System.Collections;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

/*
//Only left here for testing. Worked with a different metadata url.
public class OpenSeaMetadata
{
    [JsonProperty("_id")]
    public int id { get; set; }

    [JsonProperty("_name")]
    public string name { get; set; }

    [JsonProperty("_description")]
    public string description { get; set; }

    [JsonProperty("_image")]
    public string image { get; set; }

    [JsonProperty("_external_link")]
    public string external_link { get; set; }    
    
}
*/

public class Attribute
{
    public string trait_type { get; set; }
    public object value { get; set; }
    public string display_type { get; set; }
}

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class OpenSeaMetadata
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    //  public List<Attribute> attributes { get; set; }
    public Attribute[] attributes { get; set; }
    public string description { get; set; }
    public string external_url { get; set; }
    public string image { get; set; }
    public string name { get; set; }
}