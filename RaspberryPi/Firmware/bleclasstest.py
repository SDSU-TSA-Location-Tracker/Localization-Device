from bluepy import btle
from bluepy.btle import Scanner, DefaultDelegate
import numpy as np

A0 = "AN-A0-NA"
E0 = "AN-E0-NA"
A4 = "AN-A4-NA"
E4 = "AN-E4-NA"



# Form lists for the anchor MAC addresses and names for each mobile node to use later
# when checking through devices and advertisement packets.
anchors = [A0, E0, A4, E4]


def getrssi(value):
    if("11" == value[0:2]):
        maleRSSI = np.int8(int("0x"+value[2:4],16))
    elif("00" == value[0:2]):
        maleRSSI = 0
    if("11" == value[4:6]):
        femaleRSSI = np.int8(int("0x"+value[6:8],16))
    elif("00" == value[4:6]):
        femaleRSSI = 0
    return maleRSSI, femaleRSSI

# Create a delegate class to receive BLE broadcast packets from the anchors.
class ScanDelegate(DefaultDelegate):
	# Initialization.
	def __init__(self):
		DefaultDelegate.__init__(self)
		
	# When we discover a BLE anchor advertisement packet, extract and send data.
	def handleDiscovery(self, dev, isNewDev, isNewData):
		rssi_m = []
		rssi_f = []
		
		if(isNewDev or isNewData):
			print("Found device!")
			i = 1
			# We cycle through each anchor found in the scan.
			for anc in anchors:
				# We cycle through each advertising data item.
				# Complete local name comes before manufacturing; check if the names match.
				for (adtype, desc, value) in dev.getScanData():
					if(desc == "Complete Local Name" and value == anc):
						print("SUCCESS! Found anchor " + str(i) + "!")
					# Otherwise, return from function.
					else:
						print("ERROR: Anchor not found. Returning.")
						return
					
					# If we haven't returned on subsequent loops, we have found the anchor
					# and can retrieve the RSSI hex string.
					if(desc == "Manufacturer"):
						mrssi, frssi = getrssi(value)
						print("SUCCESS! RSSI readings found for male and female nodes.")
						rssi_m.append(mrssi)
						rssi_f.append(frssi)
					
			
			
			# For convenience, print contents of each list.
			print('rssi_m = ' + str(rssi_m))
			print('rssi_f = ' + str(rssi_f))
			
					
			# Otherwise all readings were obtained from the same mobile node.
			# We need four inputs for the model. Otherwise, we didn't collect enough data.
			if(len(rssi_m) != 4 or len(rssi_f) != 4):
				print("ERROR: Not enough RSSI inputs. Dumping values.")
				rssi_m = []
				rssi_f = []
				return
			else:
				print("SUCCESS! We have four RSSI inputs to for the model.")

			
			print("Restarting loop...")
			
			
# Create the BLE scanner.
anchor_scanner = Scanner().withDelegate(ScanDelegate())

# Scan forever in a loop.
anchor_scanner.start()
while True:
	print("Still running...")
	anchor_scanner.process()
	
	# We only want to send updates every 10 seconds.
	time.sleep(10)
	
	
# bluepy requires root access, so start program with this command:
# sudo python3 bleclass.py
