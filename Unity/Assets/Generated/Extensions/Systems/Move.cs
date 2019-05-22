namespace Systems
{
	using Entia.Queryables;
	using Entia.Unity;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Systems.Move), Link = "Assets/Scripts/TestSystems.cs", Path = new string[] { "Systems", "Move" })]
	public static class MoveExtensions
	{
		public static ref readonly global::Components.Component1 Component1(in this global::Entia.Queryables.Read<global::Components.Component1> item) => ref item.Value;
		public static ref readonly global::Components.Component1 Component1(in this global::Entia.Queryables.Read<global::Components.Component1> item, out global::Entia.States state) => ref item.Get(out state);
	}
}