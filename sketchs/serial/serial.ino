#include "serial_protocol.h"

protocol::SerialProtocol protocolProcess(Serial);
bool is_data_proccessed;
protocol::Data data;

void serialEvent() {
  data = protocolProcess.ReadData();
  if (is_data_proccessed == false) {
    data.Command = PROTOCOL_COMMAND_COMMAND_IN_PROGRESS;
    memset(&data.Payload, 0, sizeof(data.Payload));
    protocolProcess.WriteData(data);
  } else {
    is_data_proccessed = false;
  }
  Serial.flush();
}

void setup() {
  Serial.begin(115200);
  while (!Serial) {}
}

void loop() {
  if (!is_data_proccessed) {
    if (data.Command == PROTOCOL_COMMAND_ECHO) {
      protocolProcess.WriteData(data);
    } else if (data.Command == PROTOCOL_COMMAND_SET_POINTS) {
      memset(&data.Payload, 0, sizeof(data.Payload));
      memcpy(data.Payload.Message, "POINTS SET", 10);
      data.Command = PROTOCOL_COMMAND_SET_POINTS_SUCCESS;
      protocolProcess.WriteData(data);
    } else if (data.Command == PROTOCOL_COMMAND_MESSAGE_ERROR) {
      data.Command = PROTOCOL_COMMAND_MESSAGE_ERROR;
      protocolProcess.WriteData(data);
    }
    is_data_proccessed = true;
  }
}
