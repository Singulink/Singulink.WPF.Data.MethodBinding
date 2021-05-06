using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Singulink.WPF.Data.Tests
{

    public class ViewModel
    {
        public bool Executed { get; private set; }
        public Type? ParamType { get; private set; }

        public void NoParameters()
        {
            Executed = true;
        }

        public void StringParam(string x)
        {
            Executed = true;
            ParamType = typeof(string);
        }

        public void IntParam(int x)
        {
            Executed = true;
            ParamType = typeof(int);
        }

        public void NullableIntParam(int? x)
        {
            Executed = true;
            ParamType = typeof(int?);
        }

        public void OverloadedIntString(string? x)
        {
            Executed = true;
            ParamType = typeof(string);
        }

        public void OverloadedIntString(int y)
        {
            Executed = true;
            ParamType = typeof(int);
        }

        public void OverloadedNullableIntDouble(int? x)
        {
            Executed = true;
            ParamType = typeof(int?);
        }

        public void OverloadedNullableIntDouble(double y)
        {
            Executed = true;
            ParamType = typeof(double);
        }
    }
}
