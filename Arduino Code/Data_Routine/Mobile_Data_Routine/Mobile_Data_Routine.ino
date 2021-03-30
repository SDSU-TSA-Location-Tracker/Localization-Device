#include <ArduinoBLE.h>                                                                    
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>

// OLED parameters
const byte SCREEN_WIDTH = 128;
const byte SCREEN_HEIGHT = 64;
const byte OLED_RESET = -1;

// I/O pins
const byte EXTERNAL_LED = 2;
const byte INTERNAL_LED = 13;
const byte PUSH_BUTTON_SUBMIT = 3;
const byte PUSH_BUTTON_LOC = 9;

volatile int AnchorAZeroRSSI;
volatile int AnchorAFourRSSI;
volatile int AnchorEZeroRSSI;
volatile int AnchorEFourRSSI;
volatile char currentLocation[3] = {"A0"};
volatile int locationIndex = 0;

// Variables will change:
int ledState = HIGH;         // the current state of the output pin
int buttonState;             // the current reading from the input pin
int lastSubmitState = LOW;   // the previous reading from the input pin
int lastLocationState = LOW;

// the following variables are unsigned longs because the time, measured in
// milliseconds, will quickly become a bigger number than can be stored in an int.
long unsigned int lastDebounceTime = 0;  // the last time the output pin was toggled
long unsigned int debounceDelay = 50;    // the debounce time; increase if the output flickers

char locationArray[25][3] = {"A0", "A1", "A2", "A3", "A4",
                             "B0", "B1", "B2", "B3", "B4",
                             "C0", "C1", "C2", "C3", "C4",
                             "D0", "D1", "D2", "D3", "D4",
                             "E0", "E1", "E2", "E3", "E4"
                            };

Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, OLED_RESET); // OLED object instantiation

void anchorScan(void)
{
  BLE.scan(); // scan for all BLE devices in prox
  BLEDevice remote = BLE.available();
  if (remote.localName() == "AN-A0-NA")
  {
    AnchorAZeroRSSI = remote.rssi();
  }
  if (remote.localName() == "AN-A4-NA")
  {
    AnchorAFourRSSI = remote.rssi();
  }
  if (remote.localName() == "AN-E0-NA")
  {
    AnchorEZeroRSSI = remote.rssi();
  }
  if (remote.localName() == "AN-E4-NA")
  {
    AnchorEFourRSSI = remote.rssi();
  }
}

int debounce(int pinDebounce, int lastButtonState)
{
  int reading = digitalRead(pinDebounce);

  // check to see if you just pressed the button
  // (i.e. the input went from LOW to HIGH), and you've waited long enough
  // since the last press to ignore any noise:

  // If the switch changed, due to noise or pressing:
  if (reading != lastButtonState) {
    // reset the debouncing timer
    lastDebounceTime = millis();
  }

  if ((millis() - lastDebounceTime) > debounceDelay) {
    // whatever the reading is at, it's been there for longer than the debounce
    // delay, so take it as the actual current state:

    // if the button state has changed:
    if (reading != buttonState) {
      buttonState = reading;

      if (buttonState == HIGH && pinDebounce == PUSH_BUTTON_LOC)
      {
        buttonAction('A');
        digitalWrite(INTERNAL_LED, HIGH);
        digitalWrite(EXTERNAL_LED, LOW);
      }
      else if (buttonState == HIGH && pinDebounce == PUSH_BUTTON_SUBMIT)
      {
        buttonAction('B');
        digitalWrite(EXTERNAL_LED, HIGH);
        digitalWrite(INTERNAL_LED, LOW);
      }
      else if (buttonState == LOW)
      {
        digitalWrite(EXTERNAL_LED, LOW);
        digitalWrite(INTERNAL_LED, LOW);
      }
    }
  }
  return lastButtonState = reading;
}

void buttonAction(char Mode)
{
  int i;
  switch (Mode) {
    case 'A':
      // update current location to next grid spot
      locationIndex++;
      locationIndex %= 25;
      currentLocation[0] = locationArray[locationIndex][0];
      currentLocation[1] = locationArray[locationIndex][1];
      break;
    case 'B':
      // Send the current location and rssi anchor samples over serial
      digitalWrite(EXTERNAL_LED, HIGH);
        Serial.print(currentLocation[0]);
        Serial.print(currentLocation[1]);
        Serial.print("\t");
        Serial.print(AnchorAZeroRSSI);
        Serial.print("\t");
        Serial.print(AnchorEZeroRSSI);
        Serial.print("\t");
        Serial.print(AnchorAFourRSSI);
        Serial.print("\t");
        Serial.print(AnchorEFourRSSI);
        Serial.print("\n");
      digitalWrite(EXTERNAL_LED, HIGH);
      break;
    default:
      Serial.print("Unsure of Mode Selected\n");
      break;
  }
}

void updateDisplay() {
  display.clearDisplay();
  display.setCursor(0, 0);
  display.print("Grid Location: ");
  display.print(currentLocation[0]);
  display.print(currentLocation[1]);
  display.setCursor(0, 17);
  display.print("AnchorA0: ");
  display.print(AnchorAZeroRSSI);
  display.setCursor(0, 27);
  display.print("AnchorE0: ");
  display.print(AnchorEZeroRSSI);
  display.setCursor(0, 37);
  display.print("AnchorA4: ");
  display.print(AnchorAFourRSSI);
  display.setCursor(0, 47);
  display.print("AnchorE4: ");
  display.print(AnchorEFourRSSI);
  display.display();
}

void setup() {
  Serial.begin(9600);

  // Initialize OLED display and standard image
  if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) { // Address 0x3D for 128x64
    Serial.println(F("SSD1306 allocation failed"));
    for (;;);
  }
  display.clearDisplay();
  display.setTextColor(WHITE);
  display.setTextSize(1);
  display.setCursor(0, 0);
  display.print("Grid Location: A0");
  display.setCursor(0, 17);
  display.print("AnchorA0: N/A");
  display.setCursor(0, 27);
  display.print("AnchorE0: N/A");
  display.setCursor(0, 37);
  display.print("AnchorA4: N/A");
  display.setCursor(0, 47);
  display.print("AnchorE4: N/A");
  display.display();

  // Initialize all pin modes
  pinMode(EXTERNAL_LED, OUTPUT);
  pinMode(INTERNAL_LED, OUTPUT);
  pinMode(PUSH_BUTTON_SUBMIT, INPUT);
  pinMode(PUSH_BUTTON_LOC, INPUT);

  locationIndex = 0; // starting location index will be 0 which is A0

  digitalWrite(EXTERNAL_LED, LOW);

  if (!BLE.begin())
  {
    Serial.println("Starting BLE failed");
    while (1);
  }
}


void loop() {
  anchorScan();
  lastLocationState = debounce(PUSH_BUTTON_LOC, lastLocationState);
  updateDisplay();
  lastSubmitState = debounce(PUSH_BUTTON_SUBMIT, lastSubmitState);
}
