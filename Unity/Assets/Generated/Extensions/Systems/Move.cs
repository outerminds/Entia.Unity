namespace Systems
{
	[global::Entia.Unity.GeneratedAttribute(Type = typeof(global::Systems.Move), Link = "Assets/Scripts/TestSystems.cs", Path = new string[] { "Systems", "Move" })]
	public static class MoveExtensions
	{
		public static global::Entia.Entity Entity(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>> item) => item.Value1;
		public static ref readonly global::Components.Component1 Component1(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>> item) => ref item.Value2.Value;
	}
}