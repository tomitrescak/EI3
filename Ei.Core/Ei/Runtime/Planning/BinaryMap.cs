using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Core.Runtime.Planning
{
    using System;
    using System.Text;

    public struct BinaryMap : IEquatable<BinaryMap>
    {
        public long values;
        public long dontcare;

        public static List<string> ConditionNames = new List<string>(); 

        public BinaryMap(long values, long dontcare)
        {
            this.values = values;
            this.dontcare = dontcare;
        }

        public BinaryMap(int index, bool value)
        {
            this.values = 0;
            this.dontcare = -1;
            this.Set(index, value);
        }

        #region IEquatable implementation

        public bool Equals(BinaryMap other)
        {
            long care = this.dontcare ^ -1L;
            return (this.values & care) == (other.values & care);
        }

        #endregion

        public bool Set(int idx, bool value)
        {
            this.values = value ? (this.values | (1 << idx)) : (this.values & ~(1 << idx));
            this.dontcare &= ~(1 << idx);   //set the dontcare bit to false
                                            //this.dontcare ^= (1 << idx);  //toggle dontcare bit
            return true;
        }

        public bool Set(string name, bool value)
        {
            var index = ConditionNames.FindIndex(w => w == name);
            if (index == -1)
            {
                ConditionNames.Add(name);
                index = ConditionNames.Count - 1;
            }
            return this.Set(index, value);
        }

        public override string ToString()
        {
            return this.ToString(false);
            //return string.Format("{0}-{1}", Convert.ToString(this.values, 2), Convert.ToString(this.dontcare ^ -1L, 2));
        }

        public string ToString(bool vertical = false)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < ConditionNames.Count; i++)
            {
                if ((this.dontcare & (1L << i)) == 0)
                {
                    string val = ConditionNames[i];
                    if (val == null)
                        continue;
                    string upval = val.ToUpper();
                    bool set = ((this.values & (1L << i)) != 0L);

                    sb.Append(set ? upval : val);
                    sb.Append(vertical ? "\n" : " | ");
                }
            }
            return sb.ToString();
        }

        internal bool EqualsInImportantConditions(BinaryMap goal)
        {
            throw new NotImplementedException();
        }
    }
}
