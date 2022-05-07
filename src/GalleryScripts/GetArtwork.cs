using System.Collections;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.Text;
using System.Linq;
using System.Reflection;

public class GetArtwork : Node
{
    Label titleDataLabel;
    Label descDataLabel;
    Label linkDataLabel;
    TextureRect imageTextureRect;
    string imageUrl;
    public override void _Ready()
    {
        GetNode("/root/Pong/DataNode/HTTPRequest").Connect("request_completed", this, "OnRequestCompleted");
        GetNode("/root/Pong/DataNode/viewNFTButton").Connect("pressed", this, "_on_viewNFTButton_pressed");       
        titleDataLabel = GetNode<Label>("/root/Pong/DataNode/NFTDetailsPanel/TitleData");
        descDataLabel = GetNode<Label>("/root/Pong/DataNode/NFTDetailsPanel/DescData");
        linkDataLabel = GetNode<Label>("/root/Pong/DataNode/NFTDetailsPanel/LinkData");
        imageTextureRect = GetNode<TextureRect>("/root/Pong/DataNode/ArtworkPictureFrame");

        //   GetNode("/root/Pong/DataNode/ArtworkSpawner").Connect("Token_Signal", this, "setUriText");
        GD.Print("This is the SECOND URI token: " + globalVariables.URItoken);
    }

    //public void setUriText(string tokenString)
    //{
    //    GD.Print("This is the token string: " + tokenString);
    //    globalVariables.URItoken = tokenString;
    //    GD.Print("This is the URItoken: " + globalVariables.URItoken);
    //}

    // Start is called before the first frame update
    void Start()
    {

    }


    public async void _on_viewNFTButton_pressed()
    {
        HTTPRequest httpRequest = GetNode<HTTPRequest>("HTTPRequest");
        //Only left here for testing
        //   httpRequest.Request("http://www.mocky.io/v2/5185415ba171ea3a00704eed");
        httpRequest.Request(globalVariables.URItoken);
        await Task.Delay(100);
        
    }
         
    public async void OnRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        /*
         //Using json file saved to disk instead
        Godot.File files = new Godot.File();
        files.Open("res://GalleryScripts/testData.json", Godot.File.ModeFlags.Read);

        string text = files.GetAsText();
        var jsonFile = JSON.Parse(text).Result;
        */

        //JSONParseResult errors out when used directly for the deserialisation of the sample URI 
        //(but worked with a different one), but grabbing the byte code array directly from the http request result worked instead. 
        //  JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));


        string jsonString = Encoding.UTF8.GetString(body);

        //String formatting done to skip fist 23 characters in the JSON to stop an error thrown by that metadata. Should not be necessary. More tolerant encoding and deserialization required.
        jsonString = "{\n" + string.Join(string.Empty, jsonString.Skip(23));

        GD.Print("NEW JSON STRING: " + jsonString);
        Dictionary<string, string> JsonDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString, new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        });
        GD.Print("NEW DICTIONARY STRING: " + JsonDictionary);
        OpenSeaMetadata metadata = DictionaryToObject<OpenSeaMetadata>(JsonDictionary);
        //   GD.Print(metadata.name, metadata.description, metadata.image, metadata.external_url);


        await Task.Delay(200);

     //   Node proxi = proximityData.Instance();

        titleDataLabel.Text = metadata.name;
        descDataLabel.Text = metadata.description;
        linkDataLabel.Text = metadata.external_url;

        /*
        //Only for use with Ayo's Polytag Metadata available at: ipfs://bafybeidvap5erxba7wwmzdozvkoucfpd5bjrpqkozzsmvbm6chfrjcdpde/polytag_json
        //   metadata.image = string.Join(string.Empty, metadata.image.Skip(7));
        //    imageUrl = "https://ipfs.io/ipfs/" + metadata.image;
        */

        imageUrl = metadata.image;

        //Only left here for testing
      //   imageUrl = "https://miro.medium.com/max/1400/1*mk1-6aYaf_Bes1E3Imhc0A.jpeg";
        //   StartCoroutine(GetAndSetTexture(imageUrl));
        await Task.Delay(100);
        
        load_image();
        GD.Print("This is the image url:  " + imageUrl);
    }


    async void load_image()
    {
        //LOADING IMAGE VIA HTTP request. 
        var http_request = new HTTPRequest();
        AddChild(http_request);
        http_request.Connect("request_completed", this, "_http_imagerequest_completed");

        //Perform the HTTP request. The URL below returns a PNG image.
        var http_error = http_request.Request(imageUrl);
        if (http_error != Error.Ok)
        {
            GD.Print("An error occurred in the HTTP request.");
        }
    }

    void _http_imagerequest_completed(int result, int response_code, string[] headers, byte[] body)
    {
        var image = new Image();
        var image_error = image.LoadJpgFromBuffer(body);
        if (image_error != Error.Ok)
        {
            GD.Print("An error occurred while trying to display the image.");
        }
        var texture = new ImageTexture();
        texture.CreateFromImage(image);
        texture.SetSizeOverride(imageTextureRect.RectSize); 
        imageTextureRect.Texture = texture;
    }
    
    
    //*Utility script sourced from: https://www.herlitz.io/2012/12/31/mapping-dictionary-to-typed-object-using-c/
    private static T DictionaryToObject<T>(IDictionary<string, string> dict) where T : new()
    {
        var t = new T();
        PropertyInfo[] properties = t.GetType().GetProperties();

        foreach (PropertyInfo property in properties)
        {
            if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                continue;

            KeyValuePair<string, string> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

            // Find which property type (int, string, double? etc) the CURRENT property is...
            Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;

            // Fix nullables...
            Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;

            // ...and change the type
            object newA = Convert.ChangeType(item.Value, newT);
            t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
        }
        return t;
    }
}


