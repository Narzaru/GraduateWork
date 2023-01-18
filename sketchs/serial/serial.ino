#include "serial_protocol.h"

  protocol::SerialProtocol protocolProcess(Serial);

void setup() {
  Serial.begin(115200);
  while (!Serial) {}
  protocolProcess.Loop();
}

void loop() {

}
