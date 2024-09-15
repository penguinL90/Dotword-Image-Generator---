using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dot_picture_generator.Class
{
    internal record class Dotword(string Words, DotwordType Type, int Height, int Width);

    internal enum DotwordType
    {
        TwoByTwo,
        FourByFour,
    }
}
