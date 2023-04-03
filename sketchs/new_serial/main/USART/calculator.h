#pragma once

class USARTCalculator {
public:
  static constexpr uint16_t ForAsynchronousNormalMode(uint32_t f_cpu, uint32_t baud_rate) {
    /*formula taken from another library*/
    return ((f_cpu / (8L * baud_rate)) - 1) / 2;
  }

  /*NOT TESTED, may not work correctly*/
  static constexpr uint16_t ForAsynchronousDoubleSpeed(uint32_t f_cpu, uint32_t baud_rate) {
    return roundf((float)f_cpu / 8 / baud_rate - 1);
  }

  /*NOT TESTED, may not work correctly*/
  static constexpr uint16_t ForSynchronousMasterMode(uint32_t f_cpu, uint32_t baud_rate) {
    return roundf((float)f_cpu / 2 / baud_rate - 1);
  }
};