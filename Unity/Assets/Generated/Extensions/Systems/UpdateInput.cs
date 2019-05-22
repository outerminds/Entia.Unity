namespace Systems
{
	using Entia.Queryables;
	using Entia.Unity;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Systems.UpdateInput), Link = "Assets/Scripts/Example/Systems.cs", Path = new string[] { "Systems", "UpdateInput" })]
	public static class UpdateInputExtensions
	{
		public static ref global::Components.Input Input(in this global::Entia.Queryables.Write<global::Components.Input> item) => ref item.Value;
		public static ref global::Components.Input Input(in this global::Entia.Queryables.Write<global::Components.Input> item, out global::Entia.States state) => ref item.Get(out state);
		public static void Input(in this global::Entia.Queryables.Write<global::Components.Input> item, in global::Components.Input value) => item.Value = value;
	}
}