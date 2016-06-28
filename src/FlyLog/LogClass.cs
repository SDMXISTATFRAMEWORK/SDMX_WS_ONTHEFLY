using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnTheFlyLog
{
    class LogClass: ICloneable
    {
        public object _Control { get; set; }
        public FlyLog.LogTypeEnum _LogType  { get; set; }
        public string _LogText { get; set; }
        public object[] _LogOptionsText { get; set; }

        public object Clone()
        {
            return new LogClass
            {
                _Control = this._Control,
                _LogType = this._LogType,
                _LogText = this._LogText,
                _LogOptionsText = this._LogOptionsText,
            };
        }
    }
}
