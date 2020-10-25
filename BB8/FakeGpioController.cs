using System.Collections;
using System.Collections.Generic;
using Unosquare.RaspberryIO.Abstractions;

namespace BB8
{

    internal class FakeGpioController : IGpioController
    {
        public IGpioPin this[int bcmPinNumber] => throw new System.NotImplementedException();

        public IGpioPin this[BcmPin bcmPin] => throw new System.NotImplementedException();

        public IGpioPin this[P1 pinNumber] => throw new System.NotImplementedException();

        public IGpioPin this[P5 pinNumber] => throw new System.NotImplementedException();

        public int Count => throw new System.NotImplementedException();

        public IEnumerator<IGpioPin> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}