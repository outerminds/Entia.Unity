namespace Systems
{
	using Entia.Queryables;
	using Entia.Unity;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Systems.DrawVelocity), Link = "Assets/Scripts/Example/Editor/Systems.cs", Path = new string[] { "Systems", "DrawVelocity" })]
	public static class DrawVelocityExtensions
	{
		public static global::UnityEngine.Transform Transform(in this global::Entia.Queryables.All<global::Entia.Queryables.Unity<global::UnityEngine.Transform>, global::Entia.Queryables.Read<global::Components.Velocity>> item) => item.Value1.Value;
		public static ref readonly global::Components.Velocity Velocity(in this global::Entia.Queryables.All<global::Entia.Queryables.Unity<global::UnityEngine.Transform>, global::Entia.Queryables.Read<global::Components.Velocity>> item) => ref item.Value2.Value;
		public static ref readonly global::Components.Velocity Velocity(in this global::Entia.Queryables.All<global::Entia.Queryables.Unity<global::UnityEngine.Transform>, global::Entia.Queryables.Read<global::Components.Velocity>> item, out global::Entia.States state) => ref item.Value2.Get(out state);
	}
}