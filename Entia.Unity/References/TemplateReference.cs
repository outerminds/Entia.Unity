//using UnityEngine;

//namespace Entia.Unity
//{
//	[RequireComponent(typeof(EntityReference))]
//	public sealed class TemplateReference : MonoBehaviour
//	{
//		public bool Pure;
//		public bool Pool;

//		public World World => _world;
//		public Template Template => _template;
//		public Entity Entity => _entity;

//		World _world;
//		Entity _entity;
//		Template _template;

//		void Awake()
//		{
//			//if (Ensure()) _template = _world.Templaters().Template(_entity);
//		}

//		bool Ensure()
//		{
//			if (_world == null) WorldRegistry.TryGet(gameObject.scene, out _world);
//			if (_entity == Entity.Zero) _entity = GetComponent<EntityReference>()?.Entity ?? Entity.Zero;
//			return _world != null && _entity != Entity.Zero;
//		}
//	}
//}
