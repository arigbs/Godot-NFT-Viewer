using System.Collections;
using System.Collections.Generic;
using Godot;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

public class QueryArtMarket : Node
{

    private Web3 web3 = new Web3(BlockchainConstants.INFURA_GATEWAY);

    // Function Messages for itemCounter
    [Function("itemCounter","uint256")]
    public class ItemCounterFunctionMessage : FunctionMessage
    {
    }

    // Function Message for marketItems
    [Function("getMarketItem")]
    public class MarketItemsFunctionMessage : FunctionMessage
    {
        [Parameter("uint256", "itemId", 1)]
        public BigInteger itemId { get; set; }
    }

    // Function Output for marketItems (Non-Premitive Type)
    [FunctionOutput]
    public class MarketItemDTO : IFunctionOutputDTO
    {
        [Parameter("uint256","itemId",1)]
        public BigInteger itemId { get; set; }

        [Parameter("address", "nftContractAddress", 2)]
        public string nftContractAddress { get; set; }

        [Parameter("uint256", "tokenId", 3)]
        public BigInteger tokenId { get; set; }

        [Parameter("address", "seller", 4)]
        public string seller { get; set; }

        [Parameter("address", "owner", 5)]
        public string owner { get; set; }

        [Parameter("uint256","price",6)]
        public BigInteger price { get; set; }

        [Parameter("bool", "isSold", 7)]
        public bool isSold { get; set; }
    }

    // Public Callable Functions

    // Functio to get itemCounter
    public async Task<BigInteger> GetItemCounter()
    {
        var itemCounterMessage = new ItemCounterFunctionMessage();
        var queryHandler = web3.Eth.GetContractQueryHandler<ItemCounterFunctionMessage>();
        var itemCounter = await queryHandler
            .QueryAsync<BigInteger>(BlockchainConstants.ARTMARTKET_CONTRACT_ADDRESS, itemCounterMessage)
            .ConfigureAwait(false);

        return itemCounter;
    }

    // Function to get a Market Item
    public async Task<MarketItemDTO> GetMarketItem(BigInteger itemId)
    {
        var marketItemMessage = new MarketItemsFunctionMessage() { itemId = itemId };
        var queryHandler = web3.Eth.GetContractQueryHandler<MarketItemsFunctionMessage>();
        var marketItem = await queryHandler
            .QueryDeserializingToObjectAsync<MarketItemDTO>(marketItemMessage, BlockchainConstants.ARTMARTKET_CONTRACT_ADDRESS)
            .ConfigureAwait(false);

        return marketItem;
    }

    // Function to get all present Market Items
    public async Task<List<MarketItemDTO>> GetAllMarketItems()
    {
        List<MarketItemDTO> allMarketItems = new List<MarketItemDTO>();

        var itemCounter = await GetItemCounter();

        for(int i=0; i<itemCounter; i++)
        {
            var marketItem = await GetMarketItem(i);
            allMarketItems.Add(marketItem);
        }

        return allMarketItems;
    }
}
