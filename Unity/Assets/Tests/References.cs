using Entia;
using Entia.Core;
using Entia.Modules;
using Entia.Unity;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class References
{
	static (GameObject gameObject, T component, Scene scene) CreateObject<T>(bool active) where T : Component
	{
		var gameObject = new GameObject(typeof(T).Format());
		gameObject.SetActive(active);
		var component = gameObject.AddComponent<T>();
		return (gameObject, component, gameObject.scene);
	}

	static void CheckLiveEntity(IWorldReference world, IEntityReference entity)
	{
		Assert.NotNull(entity.World, "entity.World != null");
		Assert.AreEqual(entity.World, world.World, "entity.World == world.World");
		Assert.AreNotEqual(entity.Entity, Entity.Zero, "entity.Entity != Entity.Zero");
		Assert.True(entity.World.Entities().Has(entity.Entity), "entity.World.Entities().Has(entity.Entity)");
		Assert.True(EntityRegistry.Has(entity), "EntityRegistry.Has(entity)");
		Assert.True(EntityRegistry.Has(entity.World, entity.Entity), "WorldRegistry.Has(entity.World, entity.Entity)");
		Assert.AreEqual(1, EntityRegistry.References.Count(), "EntityRegistry.References.Count() == 1");
		Assert.True(EntityRegistry.References.Contains(entity), "EntityRegistry.References.Contains(entity)");
	}

	static void CheckDeadEntity(IEntityReference entity)
	{
		Assert.Null(entity.World, "entity.World == null");
		Assert.AreEqual(entity.Entity, Entity.Zero, "entity.Entity == Entity.Zero");
		Assert.False(EntityRegistry.Has(entity), "EntityRegistry.Has(entity).Not()");
		Assert.True(EntityRegistry.References.None(), "EntityRegistry.References.None()");
	}

	static void CheckLiveWorld(IWorldReference world, Scene scene)
	{
		Assert.NotNull(world.World);
		Assert.True(WorldRegistry.Has(world), "WorldRegistry.Has(world)");
		Assert.True(WorldRegistry.Has(scene), "WorldRegistry.Has(scene)");
		Assert.AreEqual(1, WorldRegistry.References.Count(), "WorldRegistry.References.Count() == 1");
		Assert.True(WorldRegistry.References.Contains(world), "WorldRegistry.References.Contains(world)");
	}

	static void CheckDeadWorld(IWorldReference world, Scene scene)
	{
		Assert.Null(world.World);
		Assert.False(WorldRegistry.Has(world), "WorldRegistry.Has(world).Not()");
		Assert.False(WorldRegistry.Has(scene), "WorldRegistry.Has(scene).Not()");
		Assert.True(WorldRegistry.References.None(), "WorldRegistry.References.None()");
	}

	[UnityTest]
	public IEnumerator HasWorldActive()
	{
		var world = CreateObject<WorldReference>(true);

		CheckLiveWorld(world.component, world.scene);

		Object.Destroy(world.gameObject);
		yield return null;

		CheckDeadWorld(world.component, world.scene);
	}

	[UnityTest]
	public IEnumerator HasWorldInactive()
	{
		var world = CreateObject<WorldReference>(false);

		CheckDeadWorld(world.component, world.scene);
		world.gameObject.SetActive(true);
		CheckLiveWorld(world.component, world.scene);

		Object.Destroy(world.gameObject);
		yield return null;

		CheckDeadWorld(world.component, world.scene);
	}

	[UnityTest]
	public IEnumerator HasEntityActive()
	{
		var world = CreateObject<WorldReference>(true);
		var entity = CreateObject<EntityReference>(true);

		CheckLiveEntity(world.component, entity.component);

		Object.Destroy(world.gameObject);
		Object.Destroy(entity.gameObject);
		yield return null;

		CheckDeadEntity(entity.component);
	}

	[UnityTest]
	public IEnumerator HasEntityInactive()
	{
		var world = CreateObject<WorldReference>(false);
		var entity = CreateObject<EntityReference>(false);

		CheckDeadEntity(entity.component);
		world.gameObject.SetActive(true);
		entity.gameObject.SetActive(true);
		CheckLiveEntity(world.component, entity.component);

		Object.Destroy(world.gameObject);
		Object.Destroy(entity.gameObject);
		yield return null;

		CheckDeadEntity(entity.component);
	}

	[UnityTest]
	public IEnumerator HasOrphanEntity()
	{
		var entity = CreateObject<EntityReference>(true);
		CheckDeadEntity(entity.component);

		var world = CreateObject<WorldReference>(true);
		CheckLiveEntity(world.component, entity.component);

		Object.Destroy(world.gameObject);
		Object.Destroy(entity.gameObject);
		yield return null;

		CheckDeadEntity(entity.component);
	}
}