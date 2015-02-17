# EZT-Exporter of Dearly Stars
# by Superfranci99

# convert a list of  bytes to int in littleEndian
def toInt(byteArray):
    result = 0
    for i in range(len(byteArray)):
        result |= (byteArray[i] << (i * 8))
    return result

# open files
fileT = open("F_SCN.IDX", "rb") # EZT -> fileT -> pointers
fileP = open("F_SCN.BIN", "rb") # EZP -> fileP -> data
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
    out = open("Data\%s" % (fileName), "wb")
    out.write(data)
    out.close()
    
