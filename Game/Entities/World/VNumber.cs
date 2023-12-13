using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World.Entities;

/// <summary>
/// Reimplementation of the c_fixed / c_fixed_10 classes
/// </summary>
public class VNumber
{
    const int factor = 10000;

    public long Value;

    public float AsFloat => Convert.ToSingle(Value / factor);
    
    public double AsDouble => Convert.ToDouble(Value / factor);

    public long AsInt64 => Convert.ToInt64(Value / factor);

    public VNumber() => Value = 0;

    public VNumber(int _value) => Value = _value * factor;

    public VNumber(uint _value) => Value = _value * factor;

    public VNumber(long _value) => Value = _value * factor;

    public VNumber(ulong _value) => Value = Convert.ToInt64(_value * factor);

    public VNumber(float _value) => Value = Convert.ToInt64(_value * factor);

    public VNumber(double _value) => Value = Convert.ToInt64(_value * factor);

    public VNumber(VNumber _number) => Value = _number.Value;

    public static long operator +(VNumber lh, VNumber rh) => lh.Value += rh.Value;

    public static long operator +(VNumber lh, long rh) => lh.Value += rh;

    public static VNumber operator ++(VNumber lh) => new(lh.Value + 1);

    public static VNumber operator -(VNumber lh, VNumber rh) => new(lh.Value -= rh.Value);

    public static long operator -(VNumber lh, int rh) => lh.Value - rh;

    public static VNumber operator --(VNumber lh) => new(lh.Value - 1);

    public static VNumber operator *(VNumber lh, VNumber rh) => new(lh.Value * rh.Value / factor);

    public static long operator *(VNumber lh, int rh) => lh.Value * rh;

    public static VNumber operator /(VNumber lh, VNumber rh) => new(lh.Value * factor / rh.Value);

    public static implicit operator VNumber(long value) => new(value);
}