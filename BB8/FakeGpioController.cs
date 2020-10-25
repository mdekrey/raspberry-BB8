using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unosquare.RaspberryIO.Abstractions;

namespace BB8
{

    internal class FakeGpioController : IGpioController
    {
        readonly IReadOnlyList<IGpioPin> pins = Enumerable.Range(0, 32).Select(v => new FakeGpioPin { BcmPinNumber = v }).ToArray();

        public IGpioPin this[int bcmPinNumber] => pins[bcmPinNumber];

        public IGpioPin this[BcmPin bcmPin] => this[(int)bcmPin];

        public IGpioPin this[P1 pinNumber] => this[(int)pinNumber];

        public IGpioPin this[P5 pinNumber] => this[(int)pinNumber];

        public int Count => pins.Count;

        public IEnumerator<IGpioPin> GetEnumerator() => pins.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => pins.GetEnumerator();
    }
}