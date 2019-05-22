namespace Systems
{
	using Entia.Queryables;
	using Entia.Unity;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Systems.UpdatePosition), Link = "Assets/Scripts/Example/Systems.cs", Path = new string[] { "Systems", "UpdatePosition" })]
	public static class UpdatePositionExtensions
	{
		public static global::UnityEngine.Transform Transform(in this global::Entia.Queryables.All<global::Entia.Queryables.Unity<global::UnityEngine.Transform>, global::Entia.Queryables.Write<global::Components.Velocity>> item) => item.Value1.Value;
		public static ref global::Components.Velocity Velocity(in this global::Entia.Queryables.All<global::Entia.Queryables.Unity<global::UnityEngine.Transform>, global::Entia.Queryables.Write<global::Components.Velocity>> item) => ref item.Value2.Value;
		public static ref global::Components.Velocity Velocity(in this global::Entia.Queryables.All<global::Entia.Queryables.Unity<global::UnityEngine.Transform>, global::Entia.Queryables.Write<global::Components.Velocity>> item, out global::Entia.States state) => ref item.Value2.Get(out state);
		public static void Velocity(in this global::Entia.Queryables.All<global::Entia.Queryables.Unity<global::UnityEngine.Transform>, global::Entia.Queryables.Write<global::Components.Velocity>> item, in global::Components.Velocity value) => item.Value2.Value = value;
	}
}