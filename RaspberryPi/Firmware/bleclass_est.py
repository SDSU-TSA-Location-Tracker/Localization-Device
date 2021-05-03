from bluepy import btle
from bluepy.btle import Scanner, DefaultDelegate
from joblib import load
from sklearn import preprocessing
from datetime import datetime

import time
import pickle
import numpy as np
import paho.mqtt.client as mqtt

A0 = "AN-A0-NA"
E0 = "AN-E0-NA"
A4 = "AN-A4-NA"
E4 = "AN-E4-NA"

anchors = [A0, E0, A4, E4]

rssi_m = [0, 0, 0, 0]
rssi_f = [0, 0, 0, 0]

pred_m = 0
pred_f = 0

maleReady = False
femaleReady = False

# Load prebuilt model using pickle.
model = load('rfc_small.pkl')

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

# Get today's date.
date = datetime.today()
datestr = date.strftime("%d_%m_%Y")
# Get current time.
now = datetime.now()
nowstr = now.strftime("%H_%M_%S")

# Create file for the current date and time.
f = open(datestr + "_" + nowstr + ".txt", "w")

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
    
    
def estimatedistance(rssi):
  n = 2
  distance = 10 ** ( (-64.2 - rssi) / (10*n) )
  return distance

def estimatexy(d4, d1, d3, d2):
  x1 = 0
  y1 = 0
  x2 = 10
  y2 = 10

  x = 0.5 * (x1 + x2 - ( ( (d1**2 + d4**2) - (d2**2 + d3**2) ) / (2*(x1-x2))))
  y = 0.5 * (y1 + y2 - ( ( (d1**2 + d2**2) - (d3**2 + d4**2) ) / (2*(y1-y2))))

  return x, y

def estimatelocation(x, y):
  loc = ""
  if(x > 0 and x < 2 and y > 8 and y < 10):
    loc = "A0"
  elif(x > 2 and x < 4 and y > 8 and y < 10):
    loc = "A1"
  elif(x > 4 and x < 6 and y > 8 and y < 10):
    loc = "A2"
  elif(x > 6 and x < 8 and y > 8 and y < 10):
    loc = "A3"
  elif(x > 8 and x < 10 and y > 8 and y < 10):
    loc = "A4"
  elif(x > 0 and x < 2 and y > 6 and y < 8):
    loc = "B0"
  elif(x > 2 and x < 4 and y > 6 and y < 8):
    loc = "B1"
  elif(x > 4 and x < 6 and y > 6 and y < 8):
    loc = "B2"
  elif(x > 6 and x < 8 and y > 6 and y < 8):
    loc = "B3"
  elif(x > 8 and x < 10 and y > 6 and y < 8):
    loc = "B4"
  elif(x > 0 and x < 2 and y > 4 and y < 6):
    loc = "C0"
  elif(x > 2 and x < 4 and y > 4 and y < 6):
    loc = "C1"
  elif(x > 4 and x < 6 and y > 4 and y < 6):
    loc = "C2"
  elif(x > 6 and x < 8 and y > 4 and y < 6):
    loc = "C3"
  elif(x > 8 and x < 10 and y > 4 and y < 6):
    loc = "C4"
  elif(x > 0 and x < 2 and y > 2 and y < 4):
    loc = "D0"
  elif(x > 2 and x < 4 and y > 2 and y < 4):
    loc = "D1"
  elif(x > 4 and x < 6 and y > 2 and y < 4):
    loc = "D2"
  elif(x > 6 and x < 8 and y > 2 and y < 4):
    loc = "D3"
  elif(x > 8 and x < 10 and y > 2 and y < 4):
    loc = "D4"
  elif(x > 0 and x < 2 and y > 0 and y < 2):
    loc = "E0"
  elif(x > 2 and x < 4 and y > 0 and y < 2):
    loc = "E1"
  elif(x > 4 and x < 6 and y > 0 and y < 2):
    loc = "E2"
  elif(x > 6 and x < 8 and y > 0 and y < 2):
    loc = "E3"
  elif(x > 8 and x < 10 and y > 0 and y < 2):
    loc = "E4"
  return loc

def locationtrack(rssi_list):
  loc = ""
  a0 = estimatedistance(rssi_list[0])
  e0 = estimatedistance(rssi_list[1])
  a4 = estimatedistance(rssi_list[2])
  e4 = estimatedistance(rssi_list[3])

  x, y = estimatexy(a0, e0, a4, e4)
  loc = estimatelocation(x, y)

  return loc

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
		
		global pred_m
		global pred_f
		
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
			
			pred_m = locationtrack(rssi_m)
			pred_f = locationtrack(rssi_f)
			
			
			print('Prediction (male): ' + pred_m)
			print('Prediction (female): ' + pred_f)
			
			# Only write prediction when we have useful values.
			if(all(rssi_m) and all(rssi_f)):
				# Write prediction to file.
				f.write(pred_m + "		" + pred_f + "\n")
				f.flush()
				
				print("PUBLISHING")
				
				#(rc, mid) = client.publish("female", str(pred_f), qos=2)
				
			
			print("Restarting loop...")
			
			
# Create the BLE scanner.
anchor_scanner = Scanner().withDelegate(ScanDelegate())

# Scan forever in a loop.
anchor_scanner.start()
while True:
	anchor_scanner.scan(10)
	
	# After 10 second scan duration, we check RSSI lists to see if they are filled with 
	# useful values. If so, publish the prediction it made. If not, send garbage values
	# so that the GUI can filter them out.
	
	if(all(rssi_m) and all(rssi_f)):
		(rc, mid) = client.publish("male", str(pred_m+"		"+pred_f), qos=2)
	
	if(not maleReady):
		if(all(rssi_m)):
			maleReady = True
			print("Publish male prediction...")
			
		else:
			print("ERROR: All 4 Male RSSI values are not currently present.")
			#(rc, mid) = client.publish("male", "xx", qos=2)
	if(maleReady):
		print("All 4 Male RSSI values are present!")
		
	if(not femaleReady):
		if(all(rssi_f)):
			femaleReady = True
			print("Publishing female prediction...")
			
		else:
			print("ERROR: All 4 Female RSSI values are not currently present.")
			#(rc, mid) = client.publish("female", "xx",  qos=2)
	if(femaleReady):
		print("All 4 Female RSSI values are present!")
			
	print("Still running...")
	
	
	
# bluepy requires root access, so start program with this command:
# sudo python3 bleclass_est.py
			
