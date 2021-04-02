from bluepy import btle
from bluepy.btle import Scanner, DefaultDelegate
from joblib import load
from sklearn import preprocessing
from datetime import datetime

import time
import pickle
import numpy as np
import pandas as pd
import paho.mqtt.client as mqtt

# MAC addresses for each anchor.
anchor1 = "dummy1"
anchor2 = "dummy2"
anchor3 = "dummy3"
anchor4 = "dummy4"

# Indexes for each mobile node
mobile1 = "00"
mobile2 = "01"

# Form lists for the anchor MAC addresses and names for each mobile node to use later
# when checking through devices and advertisement packets.
anchors = [anchor1, anchor2, anchor3, anchor4]
mobiles = [mobile1, mobile2]

# Load prebuilt model using pickle.
model = load('model.pkl')

# Recreate encoder to decode our predictions later on.
encoder = preprocessing.LabelEncoder()
classes = ["A0", "A1", "A2", "A4", "B0", "B1", "B2", "B3", "B4", "C0", "C1", "C2", "C3", "C4", "D0", "D1", "D2", "D3", "D4", "E0", "E1", "E2", "E3", "E4"]
encoder.fit(classes)

# Necessary publish function for MQTT.
def on_publish(client, userdata, mid):
	print("mid: " + str(mid))
	

# Create connection to MQTT broker.
client = mqtt.Client()
client.on_publish = on_publish
client.connect("localhost", 1883)
client.loop_start()

# Create a delegate class to receive BLE broadcast packets from the anchors.
class ScanDelegate(DefaultDelegate):
	# Initialization.
	def __init__(self):
		DefaultDelegate.__init__(self)
		
	# When we discover a BLE anchor advertisement packet, extract and send data.
	def handleDiscovery(self, dev, isNewDev, isNewData):
		name = []
		gend = []
		rssi = []
		
		
		if(isNewDev or isNewData):
			# We first look to see if the device has the same MAC address.
			for mac in anchors:
				if(dev.addr == mac):
					print("SUCCESS! Anchor MAC address is consistent.")
					# If it does, we iterate through each item in their advertisement packet.
					for (adtype, desc, value) in dev.getScanData():
						# Look for the name of the mobile node. Break out of loop if found. Otherwise we simply found a device with
						# the same MAC address of the anchor and not the anchor itself.
						if(desc == "Mobile Node Name"):
							for n in mobiles:
								if(n == val):
									print("SUCCESS! Mobile name found.")
									name.append(val)
									break;
								else:
									print("ERROR: same MAC addr as " + str(mac) + "but incorrect name")
									return;
						# If there was a unique service giving the mobile node name, we can be sure that this is an anchor.
						# Get the other advertisement items.
						if(desc == "RSSI reading"):
							print("SUCCESS! RSSI reading found.")
							rssi.append(val)
						if(desc == "Mobile Node Gender"):
							print("SUCCESS! Mobile gender found.")
							gend.append(val)
			
			# Get time and date of scan.
			scan_moment = datetime.now()
			time_of_scan = scan_moment.strftime("%H%M%S")
			date_of_scan = scan_moment.strftime("%d%m%y")
			
			# For convenience, print contents of each list.
			print('name = ' + str(name))
			print('rssi = ' + str(rssi))
			print('gend = ' + str(gend))
			
			# We do some error-checking to make sure our data extraction was done correctly.
			# Check the length of each list and make sure their data is concistent.
			if(len(name) == 4):
				name_temp = name[0]
				for n in name:
					if(n != name_temp):
						print("ERROR: Different mobile node name readings. Dumping values.")
						rssi = []
						name = []
						gend = []
						return;
				print("SUCCESS! Name readings are consistent.")
			else:
				print("ERROR: Not enough name readings collected. Dumping values.")
				rssi = []
				name = []
				gend = []
				return;
						
			if(len(gend) == 4):	
				gend_temp = gend[0]	
				for g in gend:
					if(ng!= gend_temp):
						print("ERROR: Different mobile node gender readings. Dumping values.")
						rssi = []
						name = []
						gend = []
						return;
				print("SUCCESS! Gender readings are consistent.")
			else:
				print("ERROR: Not enough gender readings collected. Dumping values.")
				rssi = []
				name = []
				gend = []
				return;
					
			# Otherwise all readings were obtained from the same mobile node.
			# We need four inputs for the model. Otherwise, we didn't collect enough data.
			if(len(rssi) != 4):
				print("ERROR: Not enough RSSI inputs. Dumping values.")
				rssi = []
				name = []
				gend = []
				return;
			else:
				print("SUCCESS! We have four RSSI inputs to for the model.")
				
			# Apply model and get grid classification prediction.
			print("Generating prediction...")
			encoded_pred = model.predict([rssi_input])
			pred = encoder.inverse_transform(encoded_pred)[0]
			
			# Publish the prediction using MQTT broker to GUI.
			print("Publishing prediction...")
			(rc, mid) = client.publish("topic", str(pred), qos=2)
			
			# Reset relevant lists and wait for new devices or data from the anchors.
			rssi = []
			name = []
			gend = []
			print("Restarting loop...")
			
			
			
			
			
# Create the BLE scanner.
anchor_scanner = Scanner().withDelegate(ScanDelegate())

# Scan forever in a loop.
anchor_scanner.start()
while True:
	print("Still running...")
	anchor_scanner.process()
	
	# We only want to send updates every 5 seconds.
	time.sleep(5)
	
	
# bluepy requires root access, so start program with this command:
# sudo python3 bleclass.py
			
