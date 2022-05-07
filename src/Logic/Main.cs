using Godot;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Web3;
using Timer = Godot.Timer;

public class Main : Node2D
{
    private Button restartPongButton;
    private CheckButton toggledGameButton;
    private TextureRect textureFrame;
    private Panel nftDetailsPanel;
    private Panel walletCheckPanel;
    private Label blockNumberLabel;
    private Label accBalanceLabel;
    private LineEdit accNumberEnter;

    private SimpleGoDotRpcClient _simpleRpcClient;

    // Node2D pongNode;
    Area2D pongBall;
    Web3 web3;
    public override async void _Ready()
    {
        pongBall = GetNode<Area2D>("/root/Pong/Ball");
        restartPongButton = GetNode<Button>("/root/Pong/RestartPongButton");
        GetNode("/root/Pong/WalletCheckPanel/AccNumText").Connect("text_entered", this, "_on_AccNumText_text_entered");
        GetNode("/root/Pong/pongCheckButton").Connect("toggled", this, "_on_PongCheckButton_toggled");
       // pongNode = GetNode<Node2D>("/root/Pong");
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        //This is a catch all certificate callback as currently GoDot does not properly support TLS 1.3 (i.e infura)
        //See: https://github.com/godotengine/godot-mono-builds/pull/47
        ServicePointManager.ServerCertificateValidationCallback +=
            (sender, certificate, chain, errors) =>
            {

                if ((errors & (SslPolicyErrors.None)) > 0)
                {
                    return true;
                }

                if (
                    (errors & (SslPolicyErrors.RemoteCertificateNameMismatch)) > 0 ||
                    (errors & (SslPolicyErrors.RemoteCertificateNotAvailable)) > 0
                )
                {
                    return false;
                }

                return true;
            };
        //using the custom http client of GoDot (this does not work  with https tsl1.3 ) so only valid for http
        //this enables GoDot web but currently having issues after timer
        //_simpleRpcClient = new SimpleGoDotRpcClient(new Uri("http://testchain.nethereum.com:8545"), this);

        GetNode<Timer>("BlockNumberTimer").Start();
        toggledGameButton = GetNode<CheckButton>("/root/Pong/pongCheckButton");
        textureFrame = GetNode<TextureRect>("DataNode/ArtworkPictureFrame");
        nftDetailsPanel = GetNode<Panel>("DataNode/NFTDetailsPanel");
        walletCheckPanel = GetNode<Panel>("WalletCheckPanel");
        blockNumberLabel = GetNode<Label>("WalletCheckPanel/lblBlockNumber");
        accBalanceLabel = GetNode<Label>("WalletCheckPanel/lblAccNumber");
        accNumberEnter = GetNode<LineEdit>("WalletCheckPanel/AccNumText");

        await SetBlockNumber();
    }

    public async void _on_AccNumText_text_entered(string text)
    {
        var balance = await web3.Eth.GetBalance.SendRequestAsync(text);
        var etherAmount = Web3.Convert.FromWei(balance.Value);

        accBalanceLabel.Text = "Wallet Balance: " + etherAmount;
        GD.Print("Wallet Balance: " + etherAmount);
    }

    public async void OnBlockNumberTimerTimeout()
    {
        await SetBlockNumber();
    }

    public async Task SetBlockNumber()
    {
        //using the custom godot rpc client
        //var web3 = new Web3(_simpleRpcClient);

        //using infura
        //var web3 = new Web3("https://mainnet.infura.io/v3/"yourProjectId"");

        web3 = new Web3("https://rinkeby.infura.io/v3/44c8e58f2822449e826e69f69ceecd54");

        //Using localhost testnet
        // var web3 = new Web3();
        var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
        blockNumberLabel.Text = "Block Number: " + blockNumber.Value;
        //   GD.Print("This is Web3: " + web3);
    }

    public Ball ballScript { get; set; }
    public async void _on_PongCheckButton_toggled(bool button_pressed)
    {
        _on_RestartPongButton_pressed();

        if (button_pressed == true)
        {
            textureFrame.Visible = false;
            walletCheckPanel.Visible = false;
            nftDetailsPanel.Visible = false;
            restartPongButton.Visible = true;
           // toggledGameButton.Text = "Check NFT";
            
        }
        else
        {
            walletCheckPanel.Visible = true;
            nftDetailsPanel.Visible = true;
            textureFrame.Visible = true;
            restartPongButton.Visible = false;
         //   toggledGameButton.Text = "Play Pong";
        }
    }

    public async void _on_RestartPongButton_pressed()
        {
            Node newBall = pongBall as Node;
            Ball ballScript = newBall as Ball;
            pongBall.Position = new Vector2(0, 0);
            ballScript.Reset();
            ballScript._Process(GetProcessDeltaTime());
        }

    public async void _on_contactLinkButton_pressed()
    {
        OS.ShellOpen("https://github.com/arigbs");
    }
}