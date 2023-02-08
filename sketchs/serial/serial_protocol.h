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

  protocol::Data ReadData() {
    command_ = os_.readString();
    command_len_ = command_.length();
    return translator_.FromBytes((const byte *)command_.c_str(), command_len_);
    return data_;
  }

  void WriteData(const protocol::Data &data) {
    os_.write(translator_.CreatePacket(data.Command, data.Payload), PROTOCOL_SIZE_PACKET);
  }

 private:
  HardwareSerial& os_;
  String command_;
  size_t command_len_;
  ProtocolTranslator translator_;
  Data data_;
};
}
#endif  // SERIAL_SERIAL_PROTOCOL_H_
