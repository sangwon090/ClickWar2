using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Utility
{
    public class Version
    {
        public static Version Zero
        { get; } = new Version(0, 0, 0, 0);

        //#####################################################################################

        public Version()
        {

        }

        public Version(string version)
        {
            this.Text = version;
        }

        public Version(int o___, int _o__, int __o_, int ___o)
        {
            V1 = ___o;
            V2 = __o_;
            V3 = _o__;
            V4 = o___;
        }

        //#####################################################################################

        /// <summary>
        /// _._._.*
        /// </summary>
        public int V1
        { get; set; } = 0;

        /// <summary>
        /// _._.*._
        /// </summary>
        public int V2
        { get; set; } = 0;

        /// <summary>
        /// _.*._._
        /// </summary>
        public int V3
        { get; set; } = 0;

        /// <summary>
        /// *._._._
        /// </summary>
        public int V4
        { get; set; } = 0;

        public string Text
        {
            get { return this.ToString(); }
            set
            {
                string[] digits = value.Split('.');

                if (digits.Length != 4)
                    throw new ArgumentException("version은 0.0.0.0 형식이여야 합니다.");

                this.V4 = int.Parse(digits[0]);
                this.V3 = int.Parse(digits[1]);
                this.V2 = int.Parse(digits[2]);
                this.V1 = int.Parse(digits[3]);
            }
        }

        //#####################################################################################

        public static Version FromString(string version)
        {
            return new Version(version);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", V4, V3, V2, V1);
        }

        public override bool Equals(object obj)
        {
            if (obj is Version)
            {
                Version right = obj as Version;

                return (this.V1 == right.V1
                    && this.V2 == right.V2
                    && this.V3 == right.V3
                    && this.V4 == right.V4);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //#####################################################################################

        protected static int CalculateVersionGap(Version left, Version right)
        {
            int gap = 0;

            gap = right.V4 - left.V4;
            if (gap != 0)
                return gap;

            gap = right.V3 - left.V3;
            if (gap != 0)
                return gap;

            gap = right.V2 - left.V2;
            if (gap != 0)
                return gap;

            gap = right.V1 - left.V1;
            if (gap != 0)
                return gap;


            return 0;
        }

        //#####################################################################################

        public static bool operator ==(Version left, Version right)
        {
            return (left.V1 == right.V1
                && left.V2 == right.V2
                && left.V3 == right.V3
                && left.V4 == right.V4);
        }

        public static bool operator !=(Version left, Version right)
        {
            return !(left == right);
        }

        public static bool operator >(Version left, Version right)
        {
            int gap = CalculateVersionGap(left, right);

            return (gap < 0);
        }

        public static bool operator <(Version left, Version right)
        {
            int gap = CalculateVersionGap(left, right);

            return (gap > 0);
        }

        public static bool operator >=(Version left, Version right)
        {
            int gap = CalculateVersionGap(left, right);

            return (gap <= 0);
        }

        public static bool operator <=(Version left, Version right)
        {
            int gap = CalculateVersionGap(left, right);

            return (gap >= 0);
        }
    }
}
