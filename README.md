# PNG console app
Contains code and standards for the insertion of CloudCoins into PNG.


## Goal

#### To insert, save and retreive data from the PNG file format.

## Summary of implementation

1) selectNewPNG().
`Allows the user to target an PNG from the "png" folder. `
2) getStack().
`Allows user to select the CloudCoin stack from a list of available stacks.`
3) saveCoinsToPNG().
`Inserts the CloudCoins as a value in an Ape tag item with a key of CloudCoinStack.`
4) exit().
`Searches the file for the Ape's key item (cloudcoinstack), creates a new file with the stacks original name, then writes the data to the file.`



### Overview of implementation.

    The current implementation is a console app built in VS-Code, to be upgraded to a multi-platform application.
    it serves as a proof of concept for storing CloudCoins in the meta tags of PNG files.


#### selectNewPNG()

Retrieve file-path to .png, store it as a string.

Using the TagLib-sharp library, and FilePath to encapsulate the file with the correct information.
```
TagLib.File PngFile = TagLib.File.Create(FilePath);
```

#### getStack()

Retrieve file-path to CloudCoin, store file content as a string.
```
string MyCloudCoin = System.IO.File.ReadAllText(<CloudCoinStack-Path>, Encoding.ASCII);
```

#### saveCoinsToPNG()

Checks file for APE Tag, returns the tag if it exists, creates of if none exist.
```
ApeTag = Methods.CheckApeTag(PngFile)
```
Inserts the CloudCoins as meta data in the Ape tag.


`

#### PNG Standard
![Header](./Standards/PNG_Header_Standard.png) 
[Body](./Standards/NPG_Body_Standard.png) 



