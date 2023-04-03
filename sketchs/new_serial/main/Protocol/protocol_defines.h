/// File contains defines for constants and describes data types

#pragma once

// Size of packet parts in bytes
#define PROTOCOL_SIZE_COMMAND 1
#define PROTOCOL_SIZE_FIRST_POINT_X 4
#define PROTOCOL_SIZE_FIRST_POINT_Y 4
#define PROTOCOL_SIZE_SECOND_POINT_X 4
#define PROTOCOL_SIZE_SECOND_POINT_Y 4
#define PROTOCOL_SIZE_MESSAGE 16
#define PROTOCOL_SIZE_PACKET 17

// Defenition protocol commands
#define PROTOCOL_COMMAND_MESSAGE '!'
#define PROTOCOL_COMMAND_ECHO 'W'
#define PROTOCOL_COMMAND_SET_POINTS 'S'
#define PROTOCOL_COMMAND_SET_POINTS_SUCCESS 's'
#define PROTOCOL_COMMAND_SET_POINTS_FAILD 'f'
#define PROTOCOL_COMMAND_MOVE 'M'
#define PROTOCOL_COMMAND_MOVE_COMPLETED 'm'
#define PROTOCOL_COMMAND_MOVE_FAILD 'x'
#define PROTOCOL_COMMAND_MESSAGE_ERROR 'E'
#define PROTOCOL_COMMAND_COMMAND_IN_PROGRESS 'I'

// Byte's offsets in the packet
#define PROTOCOL_OFFSET_COMMAND 0x00
#define PROTOCOL_OFFSET_PAYLOAD 0x01
#define PROTOCOL_OFFSET_PAYLOAD_MESSAGE 0x01
#define PROTOCOL_OFFSET_PAYLOAD_POINTS_FIRST_POSITION_X 0x01
#define PROTOCOL_OFFSET_PAYLOAD_POINTS_FIRST_POSITION_Y 0x05
#define PROTOCOL_OFFSET_PAYLOAD_POINTS_SECOND_POSITION_X 0x09
#define PROTOCOL_OFFSET_PAYLOAD_POINTS_SECOND_POSITION_Y 0x0D

// Types
namespace protocol {
struct PointPosition {
  float X;
  float Y;
};

struct Point {
  PointPosition Position;
};

struct PointsSet {
  Point First;
  Point Second;
};

union ProtocolPayload {
  PointsSet Points;
  char Message[PROTOCOL_SIZE_MESSAGE];
};

struct Data {
  unsigned char Command;
  ProtocolPayload Payload;
};
}
