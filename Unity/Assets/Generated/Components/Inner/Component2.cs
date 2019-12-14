using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Inner.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Inner.Component2), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Inner", "Component2" })][global::UnityEngine.AddComponentMenu("Components/Inner/Components.Inner.Component2")]
	public sealed partial class Component2 : global::Entia.Unity.ComponentReference<global::Components.Inner.Component2>
	{
		public global::System.Int64 X
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.X, this._X);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.X = state, value, ref this._X);
		}
		public global::System.Int64 A
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.A, this._A);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.A = state, value, ref this._A);
		}
		public global::System.Int64 B
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.B, this._B);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.B = state, value, ref this._B);
		}
		public global::System.Int64 C
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.C, this._C);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.C = state, value, ref this._C);
		}
		public global::System.Int64 D
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.D, this._D);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.D = state, value, ref this._D);
		}
		public global::System.Int64 E
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.E, this._E);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.E = state, value, ref this._E);
		}
		public global::System.Int64 F
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.F, this._F);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.F = state, value, ref this._F);
		}
		public global::System.Int64 G
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.G, this._G);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.G = state, value, ref this._G);
		}
		public global::System.Int64 H
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.H, this._H);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.H = state, value, ref this._H);
		}
		public global::System.Int64 I
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.I, this._I);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.I = state, value, ref this._I);
		}
		public global::System.Int64 J
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.J, this._J);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.J = state, value, ref this._J);
		}
		public global::System.Int64 K
		{
			get => base.Get((ref global::Components.Inner.Component2 data, global::Entia.World world) => data.K, this._K);
			set => base.Set((ref global::Components.Inner.Component2 data, global::System.Int64 state, global::Entia.World _) => data.K = state, value, ref this._K);
		}
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(X))]
		global::System.Int64 _X;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(A))]
		global::System.Int64 _A;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(B))]
		global::System.Int64 _B;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(C))]
		global::System.Int64 _C;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(D))]
		global::System.Int64 _D;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(E))]
		global::System.Int64 _E;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(F))]
		global::System.Int64 _F;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(G))]
		global::System.Int64 _G;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(H))]
		global::System.Int64 _H;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(I))]
		global::System.Int64 _I;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(J))]
		global::System.Int64 _J;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(K))]
		global::System.Int64 _K;
		public override global::Components.Inner.Component2 Raw
		{
			get => new global::Components.Inner.Component2
			{
				X = this._X,
				A = this._A,
				B = this._B,
				C = this._C,
				D = this._D,
				E = this._E,
				F = this._F,
				G = this._G,
				H = this._H,
				I = this._I,
				J = this._J,
				K = this._K
			};
			set
			{
				this._X = value.X;
				this._A = value.A;
				this._B = value.B;
				this._C = value.C;
				this._D = value.D;
				this._E = value.E;
				this._F = value.F;
				this._G = value.G;
				this._H = value.H;
				this._I = value.I;
				this._J = value.J;
				this._K = value.K;
			}
		}

	}
}