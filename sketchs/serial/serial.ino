//  I'm sure no one does that.
//  I'm talking about the whole project

#include "serial_protocol.h"

SerialProtocol protocol(Serial);

void setup() {
  Serial.begin(115200);
  while (!Serial) {}
  protocol.Loop();
}
