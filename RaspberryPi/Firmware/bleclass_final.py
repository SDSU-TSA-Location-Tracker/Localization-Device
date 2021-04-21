from bluepy import btle
from bluepy.btle import Scanner, DefaultDelegate
from joblib import load
from sklearn import preprocessing

import pickle
import numpy as np
import pandas as pd
import paho.mqtt.client as mqtt

A0 = "AN-A0-NA"
E0 = "AN-E0-NA"
A4 = "AN-A4-NA"
E4 = "AN-E4-NA"

anchors = [A0, E0, A4, E4]

rssi_m = [0, 0, 0, 0]
rssi_f = [0, 0, 0, 0]

maleReady = False
femaleReady = False

# Load prebuilt model using pickle.
model = load('model.pkl')

# Recreate encoder to decode our predictions later on.
encoder = preprocessing.LabelEncoder()
classes = ["A0", "A1", "A2", "A3", "A4", "B0", "B1", "B2", "B3", "B4", "C0", "C1", "C2", "C3", "C4", "D0", "D1", "D2", "D3", "D4", "E0", "E1", "E2", "E3", "E4"]
encoder.fit(classes)

# Necessary publish function for MQTT.
def on_publish(client, userdata, mid):
	print("mid: " + str(mid))
	
# Create connection to MQTT broker.
client = mqtt.Client()
client.on_publish = on_publish
client.connect("localhost", 1883)   # We create a broker on the Pi itself.
client.loop_start()

# Get the male and female RSSI values from the unique advertisement string.
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
		# Need to identify the lists as global variables so we can work with them in the while loop
		# and in this function.
		global rssi_m
		global rssi_f
		
		# For every new device or every new advertisement from an old device:
		if(isNewDev or isNewData):
			print("Found device!")
			# We cycle through each anchor found in the scan.
			for name in anchors:
				# 0x09 = "Complete Local Name"
				# 0xFF = "Manufacturing Specfiic Data"
				# As per https://www.bluetooth.com/specifications/assigned-numbers/generic-access-profile/
				# If the device name matches an anchor's, we take their RSSI data string and decode it
				# to get the male and female RSSI measurements from that specific anchor.
				if(dev.getValueText(0x09) == name):
					print(dev.getValueText(0x09))
					rssi_m[anchors.index(name)], rssi_f[anchors.index(name)] = getrssi(dev.getValueText(0xFF))
					
			# We should have RSSI lists each with 4 values.
			print("rssi_m = %s" % rssi_m)
			print("rssi_f = %s" % rssi_f)
				
			# Apply model and get grid classification prediction.
			print("Generating predictions...")
			
			encoded_pred_m = model.predict([rssi_m])
			encoded_pred_f = model.predict([rssi_f])
			
			pred_m = encoder.inverse_transform(encoded_pred_m)[0]
			pred_f = encoder.inverse_transform(encoded_pred_f)[0]
			
			print('Prediction (male): ' + pred_m)
			print('Prediction (female): ' + pred_f)
			
			# Publish the prediction using MQTT broker to GUI.
			print("Publishing prediction...")
			(rc, mid) = client.publish("male", str(pred_m), qos=2)
			(rc, mid) = client.publish("female", str(pred_f), qos=2)
			
			# Reset relevant lists and wait for new devices or data from the anchors.
			rssi_m = [0, 0, 0, 0]
			rssi_f = [0, 0, 0, 0]
			print("Restarting loop...")
			
			
# Create the BLE scanner.
anchor_scanner = Scanner().withDelegate(ScanDelegate())

# Scan forever in a loop.
anchor_scanner.start()
while True:
	anchor_scanner.scan(10)
	
	if(not maleReady):
		if(all(rssi_m)):
			maleReady = True
		else:
			print("ERROR: All 4 Male RSSI values are not currently present.")
	if(maleReady):
		print("All 4 Male RSSI values are present!")
		
	if(not femaleReady):
		if(all(rssi_f)):
			femaleReady = True
		else:
			print("ERROR: All 4 Female RSSI values are not currently present.")
	if(femaleReady):
		print("All 4 Female RSSI values are present!")
			
	print("Still running...")
	
	
# bluepy requires root access, so start program with this command:
# sudo python3 bleclass.py
			
