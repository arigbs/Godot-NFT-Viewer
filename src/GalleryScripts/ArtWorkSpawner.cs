using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Godot;

public class ArtWorkSpawner : Node2D
{
    private List<Node2D> spawnPoints = new List<Node2D>();
    private Node2D spawnPoint;
    public GetArtwork artwork = new GetArtwork();

// Replaced with a Global Variable for fetching the tokenURI but left here for future reference
//  [Signal]
//   public delegate void Token_Signal();


    public override void _Ready()
    {
        GetNode("/root/Pong/DataNode/viewNFTButton").Connect("pressed", this, "_on_viewNFTButton_pressed");
       
        for (int i = 0; i < 5; i++)
        {
            spawnPoint = GetNode<Node2D>("../SpawnPoint");
            spawnPoints.Add(spawnPoint);
            GD.Print("SpawnPoint Name: " + spawnPoint.Name);
            GD.Print("SpawnPoint Array Index Name: " + spawnPoints[i].Name);
            GD.Print("New SpawnPoint");
        }

        spawnArtworks();
    }
    // Start is called before the first frame update
    void Start()
    {
      //  spawnArtworks();  

    }


    // Update is called once per frame
    void Update()
    {
        
    }


    public async void _on_viewNFTButton_pressed()
    {
     //   spawnArtworks();
        GD.Print("artwork spawned");
    }

    async void spawnArtworks()
    {
        // This function still needs to be extended / improved to allow iteration through the NFT metadata in the collection. Only one get loaded at the moment.
        QueryArtMarket artMarket = new QueryArtMarket();
        QueryNFT nft = new QueryNFT();

        // Get a list of all the listed Artworks
        var itemCount = await artMarket.GetItemCounter();
        var allListedArtWorks = await artMarket.GetAllMarketItems();

        var count = (itemCount<=spawnPoints.Count) ? itemCount : spawnPoints.Count;

        // Spawn the artworks
        for(int i=0; i<count; i++)
        {
            // Left here only for future reference during future updates
            // SpawnArtWork spawnedObj = new SpawnArtWork(artwork, spawnPoints[i].Transform, spawnPoints[i].Rotation);
            // GetArtwork scriptRef = spawnedObj.artwork;

            string contractAddress = allListedArtWorks[i].nftContractAddress;
            string tokenURI = await nft.GetTokenURI(allListedArtWorks[i].tokenId, contractAddress);
            
            
            GD.Print("contract address: " + contractAddress);
            GD.Print("listed artwork: " + allListedArtWorks[i]);
            GD.Print("tokenURI: " + tokenURI);
            // Replaced with a Global Variable for fetching the tokenURI but left here for future reference
            //    EmitSignal(nameof(Token_Signal), tokenURI);
            globalVariables.URItoken = tokenURI;
            GD.Print("This is the FIRST URI token: " + globalVariables.URItoken);
        }
    }



    public class SpawnArtWork
    {
        public GetArtwork artwork { get; set; }
        public Transform2D position { get; set; }
        public float rotation { get; set; }

        public SpawnArtWork(GetArtwork _artwork, Transform2D _position, float _rotation)
    {
        artwork = _artwork;
        position = _position;
        rotation =  _rotation;
    }
}
}
