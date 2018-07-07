
// using System;
// using System.Collections.Generic;
// using System.Security.Cryptography;

// namespace AddToPng
// {

//     public class CRC32 : HashAlgorithm {
//     [CLSCompliant(false)]





//         public CRC32(uint polynomial) 
//         {
//             HashSizeValue = 32;
//             _crc32Table = (uint[])_crc32TablesCache[polynomial];

//             if (_crc32Table == null) {
//                 _crc32Table = CRC32._buildCRC32Table(polynomial);
//                 _crc32TablesCache.Add(polynomial, _crc32Table);
//             }

//             Initialize();
//         }
//     private static uint[] _buildCRC32Table(uint polynomial) 
//     {
//         uint crc;
//         uint[] table = new uint[256];
//         // 256 values representing ASCII character codes.
//         for (int i = 0; i < 256; i++) 
//         {
//             crc = (uint)i;
//             for (int j = 8; j > 0; j–) 
//             {
//             if ((crc & 1) == 1)
//                 crc = (crc >> 1) ^ polynomial;
//             else
//                 crc >>= 1;
//             }

//             table[i] = crc;
//         }

//         return table;
//     }
//     protected override void HashCore(byte[] buffer, int offset, int count) 
//     {
//         for (int i = offset; i < count; i++) 
//         {
//             ulong ptr = (_crc & 0xFF) ^ buffer[i];
//             _crc >>= 8;
//             _crc ^= _crc32Table[ptr];
//         }
//     }

//     new public byte[] ComputeHash(Stream inputStream) 
//     {
//         byte[] buffer = new byte[4096];
//         int bytesRead;  
//         while ((bytesRead = inputStream.Read(buffer, 0, 4096)) > 0) 
//         {
//           HashCore(buffer, 0, bytesRead);
//         }
//         return HashFinal();
//     }

//     protected override byte[] HashFinal() 
//     {
//         byte[] finalHash = new byte[4];
//         ulong finalCRC = _crc ^ _allOnes;

//         finalHash[0] = (byte)((finalCRC >> 0) & 0xFF);
//         finalHash[1] = (byte)((finalCRC >> 8) & 0xFF);
//         finalHash[2] = (byte)((finalCRC >> 16) & 0xFF);
//         finalHash[3] = (byte)((finalCRC >> 24) & 0xFF); 
//         return finalHash;
//     }

// }