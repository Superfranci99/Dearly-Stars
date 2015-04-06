# EZT-Exporter of Dearly Stars
# by Superfranci99

# ------------------------------------------------------------------------------------
# taken from https://github.com/magical/nlzss
# thanks to magical for the code
# LZ10 algorithm
import sys
from sys import stdin, stdout, stderr, exit
from os import SEEK_SET, SEEK_CUR, SEEK_END
from errno import EPIPE
from struct import pack, unpack

__all__ = ('decompress', 'decompress_file', 'decompress_bytes',
           'decompress_overlay', 'DecompressionError')

class DecompressionError(ValueError):
    pass

def bits(byte):
    return ((byte >> 7) & 1,
            (byte >> 6) & 1,
            (byte >> 5) & 1,
            (byte >> 4) & 1,
            (byte >> 3) & 1,
            (byte >> 2) & 1,
            (byte >> 1) & 1,
            (byte) & 1)

def decompress_raw_lzss10(indata, decompressed_size, _overlay=False):
    """Decompress LZSS-compressed bytes. Returns a bytearray."""
    data = bytearray()
    it = iter(indata)
    if _overlay:
        disp_extra = 3
    else:
        disp_extra = 1
    def writebyte(b):
        data.append(b)
    def readbyte():
        return next(it)
    def readshort():
        # big-endian
        a = next(it)
        b = next(it)
        return (a << 8) | b
    def copybyte():
        data.append(next(it))
    while len(data) < decompressed_size:
        b = readbyte()
        flags = bits(b)
        for flag in flags:
            if flag == 0:
                copybyte()
            elif flag == 1:
                sh = readshort()
                count = (sh >> 0xc) + 3
                disp = (sh & 0xfff) + disp_extra
                for _ in range(count):
                    writebyte(data[-disp])
            else:
                raise ValueError(flag)
            if decompressed_size <= len(data):
                break
    if len(data) != decompressed_size:
        raise DecompressionError("decompressed size does not match the expected size")
    return data

def decompress_bytes(data):
    """Decompress LZSS-compressed bytes. Returns a bytearray."""
    header = data[:4]
    if header[0] == 0x10:
        decompress_raw = decompress_raw_lzss10
    else:
        return data
    decompressed_size, = unpack("<L", header[1:] + b'\x00')
    data = data[4:]
    return decompress_raw(data, decompressed_size)
# ------------------------------------------------------------------------------------

# convert a list of  bytes to int in littleEndian
def toInt(byteArray):
    result = 0
    for i in range(len(byteArray)):
        result |= (byteArray[i] << (i * 8))
    return result

# open files
fileT = open(sys.argv[1], "rb") # EZT -> fileT -> pointers
fileP = open(sys.argv[2], "rb") # EZP -> fileP -> data
# read EZT header
magicIdEZT = fileT.read(4)
constEZT = fileT.read(4)
fileIdEZT = fileT.read(2)
headerSizeEZT = toInt(fileT.read(2)) + 6
nFiles = toInt(fileT.read(2))
fileT.seek(headerSizeEZT, 0)
# read EZP header
magicIdEZP = fileP.read(4)
constEZP = fileP.read(4)
fileIdEZP = fileP.read(2)
headerSizeEZP = toInt(fileP.read(2)) + 6
nameOffset = toInt(fileP.read(4))
fileP.seek(headerSizeEZP, 0)
#split the pack in subfiles
for i in range(nFiles):
    # get values from EZT
    offset = toInt(fileT.read(4))
    length = toInt(fileT.read(2))
    flagCompr = toInt(fileT.read(2))
    nameRelOff = toInt(fileT.read(4))
    print(offset, length, flagCompr, nameRelOff)
    # get data from EZP
    fileP.seek(offset, 0)
    data = fileP.read(length)
    fileP.seek(nameOffset + nameRelOff, 0)
    fileName = ""
    while(True):
        c = chr(fileP.read(1)[0])
        if(c == chr(0)):
            break;
        fileName += c
    print(fileName)
    # write the file
    out = open(sys.argv[3] + str(i) + "_" + fileName, "wb")
    if(length > 0):
        data = decompress_bytes(data)
    out.write(data)
    out.close()
fileT.close()
fileP.close()
