#ifndef SERIAL_SERIAL_PROTOCOL_H_
#define SERIAL_SERIAL_PROTOCOL_H_

#include <Arduino.h>
#include <HardwareSerial.h>
#include "protocol.h"

namespace protocol {
class SerialProtocol {
 public:
  SerialProtocol(HardwareSerial& ostream)
    : os_(ostream) {}

  void Loop() {
    protocol::Data data;
    for(;;) {
      while (!WaitCommand()) {}
      data = builder_.FromBytes((const byte *)command_.c_str(), command_len_);
      ProcessPacket(data);
    }
  }

  bool WaitCommand() {
    while (os_.available() > 0) {
      command_ = os_.readString();
      command_len_ = command_.length();
      return true;
    }
    return false;
  }

  void ProcessPacket(const protocol::Data &data) {
    static protocol::Payload payload;

    if (data.command == PROTOCOL_COMMAND_ECHO) {
      os_.write(builder_.CreatePacket(PROTOCOL_COMMAND_MESSAGE, data.payload), PROTOCOL_LIMITS_PACKET_LENGTH);
    }
    if (data.command == PROTOCOL_COMMAND_MOVE) {
      memcpy(payload.message, "ISMOVED", 7);
      os_.write(builder_.CreatePacket(PROTOCOL_COMMAND_MESSAGE, payload), PROTOCOL_LIMITS_PACKET_LENGTH);
    }
    if (data.command == PROTOCOL_COMMAND_ERROR) {
      os_.write(builder_.CreatePacket(PROTOCOL_COMMAND_ERROR, data.payload), PROTOCOL_LIMITS_PACKET_LENGTH);
    }
  }

 private:
  HardwareSerial& os_;
  String command_;
  size_t command_len_;
  ProtocolBuilder builder_;
};
}
#endif  // SERIAL_SERIAL_PROTOCOL_H_
