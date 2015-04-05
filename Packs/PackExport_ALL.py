# export all EZT-EZP files
# by Superfranci99

import xml.etree.ElementTree as ET
import os

out = "FileData"

tree = ET.parse("DearlyStarsPack.xml")
root = tree.getroot()

if not os.path.exists(out):
        os.makedirs(out)

for i in root.iter("Pack"):
    eztFile = i.find("EZT").text
    ezpFile = i.find("EZP").text
    outputDir = os.path.join(out, i.find("Output").text)
    # create folder for the subfiles, named as outputDir
    if not os.path.exists(outputDir):
        os.makedirs(outputDir)
    # call from cmd "Pack_Exporter.py eztFile ezpFile outputDir"
    command = "Pack_Exporter.py _Data\%s _Data\%s %s" % (eztFile, ezpFile, outputDir)
    os.system(command)
    print("FROM: %s %s    - TO: %s" % (eztFile.ljust(12), ezpFile.ljust(12), outputDir))
