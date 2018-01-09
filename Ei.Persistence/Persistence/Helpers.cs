using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Persistence
{
    public static class Helpers
    {
        public static string FormatCode(this String str, params string[] values) {
            return string.Format(str, values).Replace("\r\n", "\r").Trim();
        }

        public static string ToId(this String str) {
            return str.Replace(" ", "");
        }

    }
}
