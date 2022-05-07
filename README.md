# Godot-NFT-Viewer
An NFT viewer that queries NFT contracts and displays metadata, using Nethereum, the C# Ethereum library.<br>
This project is a port of **@BhaskarDutta2209**'s NFT-Gallery project (https://github.com/BhaskarDutta2209/NFT-Gallery) from Unity to Godot.<br>
**See his Code walkthrough here:** https://learn.figment.io/tutorials/building-a-3d-art-gallery-using-unity3d-and-nethereum#writing-scripts-to-interact-with-the-blockchain<br>

This Godot implementation of the NFT Viewer has been implemented by Ayodele Arigbabu (@arigbs / @DADA_universe) and currently only features a 2D viewer rather than the 3D gallery demonstrated in the original project.<br> 

The Nethereum integration in Godot takes off from the "GoDot Pong with C# with Nethereum Integration" prepared shared by Nethereum main developer- **@juanfranblanco** : https://github.com/Nethereum/GoDot-Example<br>

An issue with SSL certificates is addressed in that project and no attempt has been made to address that here.<br>

This project is therefore likely to break when built in it's current form. The HTTPRequest also times out sometimes and the build / project would / may crash if the internet connection fails or there's none. When loading an NFT, give some time for the image to get downloaded and displayed, it could take some time.<br> 

The integration example includes the simple Pong game GoDot demo, and the full code and assets for that entire integration have been included here.<br>

**With this Godot project, you can:**
* Query an Ethereum wallet address for it's balance
* Query a hard coded NFT smart contract and market place for NFT metadata (only one at the moment) and retrieve the NFT's image and text.
* View a readout of the blockchain's block number (from @juanfranblanco's sample project)
* Play Pong!

![Editor view](/assets/images/tux.png)
![View when running with Metadata loaded](/assets/images/tux.png)

This project is shared to help with faster onboarding of developers seeking to interact with the Ethereum blockchain through Godot. More intricate interactions with the Ethereum blockchain are now eminently possible thanks to the Nethereum C# library. Further adaptations are most welcome. Check back at this repository for possible updates to the Godot NFT Viewer project.<br>

**To-do list as at the time of first commit include:**
* Remove need for string formatting done to skip fist 23 characters in the JSON to stop an error thrown by that metadata. Should not be necessary. More tolerant encoding   and deserialization of different metadata types should be possible
* Add progress bars and prompts for information on background processes and error handling.
* Implement iteration through all the NFTs in the collection, only one NFT's metadata is currently loaded.
* Make the Pong game more fun and maybe implement some blockchain interaction into the gameplay.
* Create a second project off this one to implement a full 3D scene similar to the one demoed in Unity by @BhassarDutta2209

**This To-do list is a wishlist, and should not be expected to adhere to any timeline.**


