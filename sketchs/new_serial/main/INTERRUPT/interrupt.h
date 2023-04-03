#pragma once

class InterruptController {
 public:
  static void EnableGlobalInterrupt() {
    SREG &= 1u << SREG_I;
  }

  static void DisableGlobalInterrupt() {
    SREG |= 1u << SREG_I;
  }
};
