#include "HardwareSerial.h"
#include "Arduino.h"
#ifndef SERIAL_PROTOCOL_H_
#define SERIAL_PROTOCOL_H_
#include <stdint.h>

// Commands
#define PACKET_SIZE 9
#define PAYLOAD_SIZE 8
#define MESSAGE '!'
#define WHO_IS 'W'
#define MOVE_TO 'M'
#define ERROR 'E'

// |command(uint8)[0:1]|position_x(float)[1:5]|position_y(float)[5:8]
// Offsets
#define COMMAND 0
#define PAYLOAD 1
#define POSITION_X 1
#define POSITION_Y 5

struct Position {
  float position_x;
  float position_y;
};

union Payload {
  Position pos;
  char msg[8];
};

struct Data {
  uint8_t type;
  Payload union_data;
};

class ProtocolBuilder {
 public:
  const byte* ToBytes(const Data &data) {
    memcpy(bytes_ + COMMAND, &data.type, 1);
    memcpy(bytes_ + POSITION_X, &data.union_data.pos.position_x, 4);
    memcpy(bytes_ + POSITION_Y, &data.union_data.pos.position_y, 4);
    return bytes_;
  }

  Data FromBytes(const byte *bytes, size_t size) {
    Data data;
    if (size != PACKET_SIZE) {
      data.type = ERROR;
      memset(data.union_data.msg, 0, 8);
      return data;
    }
    memcpy(&data.type, bytes + COMMAND, 1);
    memcpy(&data.union_data.pos.position_x, bytes + POSITION_X, 4);
    memcpy(&data.union_data.pos.position_y, bytes + POSITION_Y, 4);
    return data;
  }

  byte* CreatePacket(char type, const Payload &payload) {
    bytes_[0] = type;
    memcpy(bytes_ + PAYLOAD, (void *)payload.msg, 8);
    return bytes_;
  }

 private:
  byte bytes_[PACKET_SIZE];
};

#endif  // SERIAL_PROTOCOL_H_
