using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class LimitedFloat
{
    #region Properties

    [SerializeField]
    private float min;
    public float Min
    {
        get { return min; }
        set { min = value; }
    }

    [SerializeField]
    private float max;
    public float Max
    {
        get { return max; }
        set { max = Mathf.Clamp(value, min, float.MaxValue); }
    }

    [SerializeField]
    private float current;
    public float Current
    {
        get { return current; }
        set { current = Mathf.Clamp(value, min, max); ; }
    }

    #endregion

    #region Constructors

    public LimitedFloat(float argStartValue)
    {
        current = argStartValue;
        min = float.MinValue;
        max = float.MaxValue;
    }

    public LimitedFloat(float argStartValue, float argMin, float argMax)
    {
        current = argStartValue;
        min = argMin;
        max = argMax;
    }

    public LimitedFloat(LimitedFloat argCopy)
    {
        min = argCopy.Min;
        max = argCopy.Max;
        current = argCopy.Current;
    }

    #endregion

    #region Operator Overloads

    public static LimitedFloat operator +(LimitedFloat argLHS, float argRHS)
    {
        LimitedFloat myLimitedFloat = new LimitedFloat(argLHS);
        myLimitedFloat.Current += argRHS;
        return myLimitedFloat;
    }

    public static LimitedFloat operator -(LimitedFloat argLHS, float argRHS)
    {
        LimitedFloat myLimitedFloat = new LimitedFloat(argLHS);
        myLimitedFloat.Current -= argRHS;
        return myLimitedFloat;
    }

    #endregion
}