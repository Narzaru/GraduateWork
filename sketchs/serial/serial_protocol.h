#include "Arduino.h"
#ifndef SERIAL_SERIAL_PROTOCOL_H_
#define SERIAL_SERIAL_PROTOCOL_H_

#include <HardwareSerial.h>
#include "protocol.h"

class SerialProtocol {
 public:
  SerialProtocol(HardwareSerial& ostream)
    : os_(ostream) {}

  void Loop() {
    Data data;
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

  void ProcessPacket(const Data &data) {
    static Payload payload;

    if (data.type == WHO_IS) {
      memcpy(payload.msg, "ARDUINO\0", PAYLOAD_SIZE);
      os_.write(builder_.CreatePacket(MESSAGE, payload), PACKET_SIZE);
    } else {
      memset(payload.msg, 0, PAYLOAD_SIZE);
      os_.write(builder_.CreatePacket(ERROR, payload), PACKET_SIZE);
    }
  }

 private:
  HardwareSerial& os_;
  String command_;
  size_t command_len_;
  ProtocolBuilder builder_;
};

#endif  // SERIAL_SERIAL_PROTOCOL_H_
