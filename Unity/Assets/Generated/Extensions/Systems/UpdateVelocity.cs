namespace Systems
{
	using Entia.Queryables;
	using Entia.Unity;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Systems.UpdateVelocity), Link = "Assets/Scripts/Example/Systems.cs", Path = new string[] { "Systems", "UpdateVelocity" })]
	public static class UpdateVelocityExtensions
	{
		public static ref global::Components.Velocity Velocity(in this global::Systems.UpdateVelocity.Query item) => ref item.Velocity.Value;
		public static ref global::Components.Velocity Velocity(in this global::Systems.UpdateVelocity.Query item, out global::Entia.States state) => ref item.Velocity.Get(out state);
		public static void Velocity(in this global::Systems.UpdateVelocity.Query item, in global::Components.Velocity value) => item.Velocity.Value = value;
		public static ref readonly global::Components.Motion Motion(in this global::Systems.UpdateVelocity.Query item) => ref item.Motion.Value;
		public static ref readonly global::Components.Motion Motion(in this global::Systems.UpdateVelocity.Query item, out global::Entia.States state) => ref item.Motion.Get(out state);
		public static ref readonly global::Components.Physics Physics(in this global::Systems.UpdateVelocity.Query item) => ref item.Mass.Value;
		public static ref readonly global::Components.Physics Physics(in this global::Systems.UpdateVelocity.Query item, out global::Entia.States state) => ref item.Mass.Get(out state);
		public static ref readonly global::Components.Input Input(in this global::Systems.UpdateVelocity.Query item) => ref item.Input.Value;
		public static ref readonly global::Components.Input Input(in this global::Systems.UpdateVelocity.Query item, out global::Entia.States state) => ref item.Input.Get(out state);
		public static ref readonly global::Entia.Components.Debug Debug(in this global::Systems.UpdateVelocity.Query item) => ref item.Debug.Value;
	}
}