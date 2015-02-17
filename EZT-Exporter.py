# EZT-Exporter of Dearly Stars
# by Superfranci99

# convert a list of  bytes to int in littleEndian
def toInt(byteArray):
    result = 0
    for i in range(len(byteArray)):
        result |= (byteArray[i] << (i*8))
    return result

file = open("F_SCN.IDX", "rb")
magicId    = file.read(4)
const      = file.read(4)
fileId     = file.read(2)
headerSize = toInt(file.read(2)) + 6
nFiles     = toInt(file.read(2))
file.seek(headerSize, 0)
for i in range(nFiles):
    offset    = toInt(file.read(4))
    length    = toInt(file.read(2))
    flagCompr = toInt(file.read(2))
    nameOff   = toInt(file.read(4))
    print(offset, length, flagCompr, nameOff)
    


