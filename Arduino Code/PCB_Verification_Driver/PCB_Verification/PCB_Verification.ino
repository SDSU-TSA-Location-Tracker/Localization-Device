const int MODE_A = 10;
const int MODE_B = 9;
const int MODE_C = 8;
const int PUSH_BUTTON = 3;
const int LED_EXTERNAL = 2;
const int LED_INTERNAL_R = 22;
const int LED_INTERNAL_G = 23;
const int LED_INTERNAL_B = 24;


void setup() {
  pinMode(MODE_A, INPUT);
  pinMode(MODE_B, INPUT);
  pinMode(MODE_C, INPUT);
  pinMode(PUSH_BUTTON, INPUT);
  pinMode(LED_EXTERNAL, OUTPUT);
}

void loop() {
  if (digitalRead(MODE_A) == HIGH && digitalRead(MODE_B) == LOW && digitalRead(MODE_C) == LOW)
  {
    digitalWrite(LED_INTERNAL_R, HIGH);
    delay(500);
  }
  else if (digitalRead(MODE_B) == HIGH && digitalRead(MODE_A) == LOW && digitalRead(MODE_C) == LOW)
  {
    digitalWrite(LED_INTERNAL_G, HIGH);
    delay(500);
  }
  else if (digitalRead(MODE_C) == HIGH && digitalRead(MODE_B) == LOW && digitalRead(MODE_A) == LOW)
  {
    digitalWrite(LED_INTERNAL_B, HIGH);
    delay(500);
  }
  else
  {
    digitalWrite(LED_EXTERNAL, LOW);
    digitalWrite(LED_INTERNAL_R, LOW);
    digitalWrite(LED_INTERNAL_G, LOW);
    digitalWrite(LED_INTERNAL_B, LOW);
  }
  if (digitalRead(PUSH_BUTTON) == HIGH)
    digitalWrite(LED_EXTERNAL, HIGH);
  else
    digitalWrite(LED_EXTERNAL, LOW);

  digitalWrite(LED_INTERNAL_R, LOW);
  digitalWrite(LED_INTERNAL_G, LOW);
  digitalWrite(LED_INTERNAL_B, LOW);
  delay(250);
}
