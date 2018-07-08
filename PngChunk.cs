using System;
using Dexiom.QuickCrc32;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime;
using TagLib; //"taglib-sharp-netstandard2.0/2.1.0"

namespace AddToPng
{
    class PngChunk {

        //Chunk layout
        //[chunkLength(4 bytes), chunkType(4 bytes), chunkData(), chunkCRC()]
        //  cLDc Chunk Data
			//
			// Byte 0-2     'CC CC CC' Marks start of the coin.
			// Byte 3       1 is internal use, 0 is transfer.
			// Byte 4-12    Plaintext name of coin.
			// Byte 13-14   Designates how the bytes in the 'AN' are used.
			// Byte 15      Network number
			// Byte 16-18   Serial number
			// Byte 19-31   Last POWN results.
			// Byte 32-431  Authentication Numbers.
			// 4 Bytes      Data end Marker
            // 4 Bytes      CRC data

    public byte[] setChunk(byte[] coin)
    {//Calls other ancilliary methods to return a complete png chunk containing the data from the parameters.
        byte[] chunkEndMarker = returnEndCoin();// 4 byte  data end designator. 
        byte[] chunkLength = returnChunkLength(coin, chunkEndMarker); // chunkLength (4 bytes)
        byte[] chunkType = returnChunkType(); //chunkType (4 bytes)
        byte[] chunkCRC = returnChunkCRC(coin, chunkLength);//chunkCRC (4 bytes)
        int totalLength = coin.Length + chunkLength.Length 
        + chunkType.Length + chunkEndMarker.Length + chunkCRC.Length;
        byte[] completeChunk = new byte[totalLength];
        completeChunk = returnChunk(chunkLength,chunkType,coin,chunkEndMarker,chunkCRC,totalLength);
        return completeChunk;
    }

    public byte[] returnEndCoin()
    {//returns a 4 byte sequence ?$?$ to mark the end of the data portion of the chunk.
        byte[] endChunkSequence = new byte[] {(byte)'{', (byte)'?', (byte)'$', (byte)'}'};
        // Console.WriteLine("endSequence: " + endChunkSequence.ToString());
        // foreach(byte b in endChunkSequence){
        //     char c = (char)(b);
        //     Console.WriteLine("byte: "+c);
        // }
        return endChunkSequence;
    }

    public byte[] returnChunkLength(byte[] data, byte[] endMark){
        //A 4 byte file
        byte[] byteLength = new byte[4];
        //get length of CloudCoin data and end marker. Used long for 4 bytes (size of png chunk length specifier.).
        int length = data.Length + endMark.Length; 
        //Convert length to binary.
        byteLength = BitConverter.GetBytes(length); 

        // Console.WriteLine("chunkLength: " + length);
        // foreach(byte b in byteLength){
        //     string c = b.ToString();
        //     Console.WriteLine("byte: "+c);
        // }
        return byteLength;
    }

    public byte[] returnChunkType(){
        byte[] type = new byte[] {(byte)'c',(byte)'L',(byte)'D',(byte)'c'}; // 4 byte Chunk ID. cLDc
        // Console.WriteLine("chunkType: cLDc");
        // foreach(byte t in type){
        //     string c = t.ToString();
        //     Console.WriteLine("byte: "+c);
        // }
        return type;
    }
    public byte[] returnChunkCRC(byte[] data, byte[] type){
        int length = data.Length + type.Length; //
        byte[] chunkInfo = new byte[length];
        for(int i = 0; i < length; i++)
        {
            if(i < type.Length)
                chunkInfo[i] = type[i]; //type "cLDc"
            else
                chunkInfo[i] = data[i-type.Length];
        }
        uint crc = QuickCrc32.Compute(chunkInfo); //get the crc. 
        byte[] crcBytes = BitConverter.GetBytes(crc);
        // Console.WriteLine("chunkCRC: " + crc);
        // foreach(byte b in crcBytes){
        //     string c = b.ToString();
        //     Console.WriteLine("byte: "+c);
        // }
        return crcBytes;
    }
    public byte[] returnChunk(byte[] length, byte[] type, byte[] data,byte[] endMark, byte[] crc, int totalLength){
        byte[] ccChunk = new byte[totalLength];

        int l1 = length.Length;
        int l2 = l1 + type.Length;
        int l3 = l2 + data.Length;
        int l4 = l3 + endMark.Length;
        int l5 = l4 + crc.Length;

        for(int i = 0; i < ccChunk.Length; i++)
            {
                if(i < l1)
                    ccChunk[i] = length[i]; //first 4 bytes of chunk is length 0-3
                else if(i >= l1 && i < l2)
                    ccChunk[i] = type[i-l1]; //second 4 bytes of chunk is type 4-7
                else if(i >= l2 && i < l3)
                    ccChunk[i] = data[i-l2]; //third part is data from byte 8  to ccChunk.Length-5
                else if(i >=l3 && i < l4)
                    ccChunk[i] = endMark[i-l3];  // 4 byte end sequence
                else if(i >=l4 && i < l5)
                    ccChunk[i] = crc[i-l4];  //last 4 bytes crc
            }
        return ccChunk;
        }
    }
}
     
