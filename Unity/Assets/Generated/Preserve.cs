namespace Generated
{
	[global::Entia.Core.PreserveAttribute]
	static class Preserve
	{
		[global::Entia.Core.PreserveAttribute]
		static void Segments(global::Entia.Modules.Entities entities)
		{
			entities.Create<global::Segments.Segment1>();
			entities.Create<global::Segments.Inner.Segment2>();
			entities.Create<global::Entia.Segments.Default>();
		}
	}
}