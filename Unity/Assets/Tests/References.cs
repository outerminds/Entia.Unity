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
    }

    static void CheckDeadEntity(IEntityReference entity)
    {
        Assert.Null(entity.World, "entity.World == null");
        Assert.AreEqual(entity.Entity, Entity.Zero, "entity.Entity == Entity.Zero");
    }

    static void CheckLiveWorld(IWorldReference world, Scene scene)
    {
        Assert.NotNull(world.World);
    }

    static void CheckDeadWorld(IWorldReference world, Scene scene)
    {
        Assert.Null(world.World);
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