namespace Systems
{
	using Entia.Queryables;
	using Entia.Unity;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Systems.Queries), Link = "Assets/Scripts/TestSystems.cs", Path = new string[] { "Systems", "Queries" })]
	public static class QueriesExtensions
	{
		public static global::Entia.Entity Entity(in this global::Entia.Entity item) => item;
		public static ref readonly global::Components.Inner.Ambiguous Ambiguous(in this global::Entia.Queryables.All<global::Entia.Queryables.Read<global::Components.Inner.Ambiguous>, global::Entia.Queryables.Read<global::Components.Ambiguous>> item) => ref item.Value1.Value;
		public static ref readonly global::Components.Inner.Ambiguous Ambiguous(in this global::Entia.Queryables.All<global::Entia.Queryables.Read<global::Components.Inner.Ambiguous>, global::Entia.Queryables.Read<global::Components.Ambiguous>> item, out global::Entia.States state) => ref item.Value1.Get(out state);
	}
}