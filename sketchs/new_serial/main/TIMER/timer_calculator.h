#pragma once

#include <stdint.h>

class TimerCalculator {
public:
  /* return calculated period in msec from OCR */
  static constexpr uint32_t TimeFromOCR(int cpu_clock, int prescaler, int OCR) {
    return (prescaler * (1 + OCR)) / (cpu_clock / 1000);
  }

  /*return calculated OCR from timer in msec*/
  static constexpr uint32_t OCRFromTime(uint32_t cpu_clock, uint32_t prescaler, uint32_t msec) {
    return msec * (cpu_clock / 1000) / prescaler - 1;
  }
};
