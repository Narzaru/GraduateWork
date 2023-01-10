void setup() {
  Serial.begin(9600);
  while (!Serial) {}  // wait for open serial
  Serial.println("Initialize completed with code 0");
}

enum class command {
  kMoveTo
};

class CommandParser {
 public:
  CommandParser(HardwareSerial& stream)
    : os_(Serial),
      debug_(false),
      commands_counter_(0) {}

  void ProcessCommand(const String& command) {
    ++commands_counter_;

    if (debug_) {
      DoDebug(command);
    }

    if (command.equals("enable debug")) {
      debug_ = true;
    } else if (command.equals("detect")) {
      DoDetect();
    } else {
      DoNotFound();
    }

  }

  void DoDebug(const String& command) {
    os_.println("command #" + String(commands_counter_) + " " + command);
  }

  void DoDetect() {
    os_.println("detected");
  }

  void DoNotFound() {
    os_.println("command not found");
  }

 private:
  HardwareSerial& os_;
  bool debug_;
  unsigned commands_counter_;
};

CommandParser parser(Serial);
void loop() {
  while (Serial.available()) {
    parser.ProcessCommand(Serial.readStringUntil('\n'));
  }
}
