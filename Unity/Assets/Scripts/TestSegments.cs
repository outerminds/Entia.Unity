using Entia;
using Entia.Segments;

namespace Segments
{
    public struct Segment1 : ISegment { }

    namespace Inner
    {
        public struct Segment2 : ISegment { }
    }
}