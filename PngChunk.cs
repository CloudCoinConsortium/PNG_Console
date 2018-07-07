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
			// Followed by 4 Bytes CRC data

        byte[] chunkCRC = {0x20, 0x20, 0x20, 0x20}; // 4 byte CRC.

    public byte[] setChunk(byte[] coin){
        byte[] chunkLength = returnChunkLength(coin); // chunkLength (4 bytes)
        byte[] chunkType = returnChunkType(); //chunkType (4 bytes)
        byte[] chunkCRC = returnChunkCRC(coin, chunkLength);//chunkCRC (4 bytes)

        int totalLength = coin.Length + chunkLength.Length + chunkType.Length + chunkCRC.Length;
        byte[] completeChunk = new byte[totalLength];

        completeChunk = returnChunk(chunkLength,chunkType,coin,chunkCRC,totalLength);

        return completeChunk;
    }

    public byte[] returnChunkLength(byte[] data){
        //A 4 byte file
        byte[] byteLength = new byte[4];
        //get length of CloudCoin data. Used long for 4 bytes (size of png chunk length specifier.).
        int length = data.Length; 
        //Convert length to binary.
        Console.WriteLine("chunkLength: " + length);
        byteLength = BitConverter.GetBytes(length); 
        foreach(byte b in byteLength){
            string c = b.ToString();
            Console.WriteLine("byte: "+c);
        }
        return byteLength;
    }

    public byte[] returnChunkType(){
        byte[] type = new byte[] {99, 76, 68, 99}; // 4 byte Chunk ID. cLDc
        Console.WriteLine("chunkType: cLDc");
        foreach(byte t in type){
            string c = t.ToString();
            Console.WriteLine("byte: "+c);
        }
        return type;
    }
    public byte[] returnChunkData(byte[] coin){
        byte[] data = coin;
        return data;
    }
    public byte[] returnChunkCRC(byte[] data, byte[] type){

        int length = data.Length + type.Length;

        byte[] chunkInfo = new byte[length];
        
        for(int i = 0; i < length; i++)
        {
            if(i < type.Length)
                chunkInfo[i] = type[i];
            else
                chunkInfo[i] = data[i-type.Length];
        }
        uint crc = QuickCrc32.Compute(chunkInfo);


        byte[] crcBytes = BitConverter.GetBytes(crc);
        Console.WriteLine("chunkCRC: " + crc);
        foreach(byte b in crcBytes){
            string c = b.ToString();
            Console.WriteLine("byte: "+c);
        }
        return crcBytes;
    }
    public byte[] returnChunk(byte[] length, byte[] type, byte[] data, byte[] crc, int totalLength){
        byte[] ccChunk = new byte[totalLength];
        int l1 = length.Length;
        int l2 = l1 + type.Length;
        int l3 = l2 + data.Length;
        int l4 = l3 + crc.Length;

        for(int i = 0; i < ccChunk.Length; i++)
            {
                if(i < l1)
                    ccChunk[i] = length[i]; //first 4 bytes of chunk is length 0-3
                else if(i >= l1 && i < l2)
                    ccChunk[i] = type[i-l1]; //second 4 bytes of chunk is type 4-7
                else if(i >= l2 && i < l3)
                    ccChunk[i] = data[i-l2]; //third part is data from byte 8  to ccChunk.Length-5
                else if(i >=l3 && i < l4)
                    ccChunk[i] = crc[i-l3];  //last 4 bytes crc
            }
        return ccChunk;
        }
    }
}
     
