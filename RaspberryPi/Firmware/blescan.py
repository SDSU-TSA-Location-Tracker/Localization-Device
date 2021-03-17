from bluepy import btle
from bluepy.btle import Scanner, DefaultDelegate
from joblib import load
from sklearn.svm import SVC
from datetime import datetime

import time
import pickle
import numpy  as np
import pandas as pd

# MAC addresses for each anchor.
anchor1 = "dummy1"
anchor2 = "dummy2"
anchor3 = "dummy3"
anchor4 = "dummy4"

# Indexes for each mobile node
mobile1 = "00"
mobile2 = "01"

anchors = [anchor1, anchor2, anchor3, anchor4]
mobiles = [mobile1, mobile2]

rssi = []
name = []
gend = []

model = load('BLEclassifier.joblib')

# create a delegate class to receive the BLE broadcast packets
class ScanDelegate(DefaultDelegate):
    def __init__(self):
        DefaultDelegate.__init__(self)

    # when this python script discovers a BLE broadcast packet, print a message with the device's MAC address
    def handleDiscovery(self, dev, isNewDev, isNewData):
        if isNewDev:
			# Check each MAC address for the anchors.
			for mac in anchors:
				if (dev.addr == mac):
					# Even if the MAC address matches, it is not uniquely identifiable. Check each avertising data item.
					for (adtype, desc, value) in dev.getScanData():
						# If we find the name, check if it matches the mobile list.
						if(desc == "Mobile Node Name"):
							for n in mobiles:
								# If so, append to the name list.
								if(n == val):
									name.append(val)
									break;
								else:
									print("ERROR: Same MAC address as ", mac, ", but incorrect name. This is not an anchor.")
									return;
						# If it does match, continue adding relevant data to their lists.
						if(desc == "RSSI reading"):
							rssi.append(val)
						if(desc == "Mobile Node Gender"):
							gend.append(val)
			
			# Get the time of the scan.				
			scan_moment = datetime.now()
			time_of_scan = str(now.scan_moment("%H%M%S"))
			date_of_scan = str(now.scan_moment("%d%m%y"))
                
			name_temp = name[0]
			gend_temp = gend[0]
			
			for n, g in zip(name, gend):
				if(n != name_temp):
					print("ERROR: Different mobile node name readings.")
					rssi = []
					name = []
					gend = []
					return;
				if(g != gend_temp):
					print("ERROR: Different mobile node gender readings.")
					rssi = []
					name = []
					gend = []
					return;
			
			# Otherwise all readings were obtained from same mobile node.
			if(len(rssi)) == 4:
				rssi_input = np.array(rssi)
			else:
				print("ERROR: RSSI list not in correct format.")
				rssi = []
				name = []
				gend = []
				return;
			
			# Apply the model to the RSSI input and get grid coordinate prediction.
			pred = model.predict(rssi_input)
			coordinate = str(pred[0])
			#coordinate = "haha"
			
			# Append the relevant data to the log.
			location_update = [coordinate + "\n"]
			# File name = M_05132021_123024.txt
			log = open(gend[0] + "_" + date_of_scan + "_" + time_of_scan + ".txt", "a")
			log.write(location_update)
			log.close()
			
			# For real-time location, send data to GUI program through pipe.
			p = Popen(csharpexec, stdout=PIPE, stdin=PIPE, stderr=PIPE)
			out, err = p.communicate(coordinate)
			
			# Reset the relevant data lists.
			rssi = []
			name = []
			gend = []
			
				
anchor_scanner = Scanner.withDelegate(ScanDelegate())

while True:
	print ("Still running...")
	anchor_scanner.process()
		