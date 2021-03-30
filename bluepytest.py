from bluepy import btle
from bluepy.btle import Scanner, DefaultDelegate

# create a delegate class to receive the BLE broadcast packets
class ScanDelegate(DefaultDelegate):
    def __init__(self):
        DefaultDelegate.__init__(self)

        # when this python script discovers a BLE broadcast packet, print a message with the device's MAC address
        def handleDiscovery(self, dev, isNewDev, isNewData):
            if isNewDev:
                print("Discovered device:", dev.addr)
                #if(dev.addr == "mac add"):
                for(adtype, desc, value) in dev.getScanData():
                        print(desc, " = ", value)
            elif isNewData:
                print("Received new data from", dev.addr)
                for (adtype, desc, value) in dev.getScanData():
                    print(desc, " = ", value)

# create a scanner object that sends BLE broadcast packets to the ScanDelegate
scanner = Scanner().withDelegate(ScanDelegate())

# start the scanner and keep the process running
scanner.start()
while True:
    print("Still running...")
    scanner.process()
